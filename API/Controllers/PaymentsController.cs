using System;
using System.IO;
using System.Threading.Tasks;
using API.Extensions;
using API.SignalR;
using core.Entities;
using core.Entities.OrderAggregate;
using core.Interface;
using core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;
using OrderEntity = core.Entities.OrderAggregate.Order;

namespace API.Controllers;

public class PaymentsController : BaseApiController
{
    private readonly IPaymentService _paymentService;
    private readonly IUnitOfWork _unit;
    private readonly ILogger<PaymentsController> _logger;
    private readonly IHubContext<NotificationHub> _hubContext; // <-- store it here

    private readonly string _whSecret;

    public PaymentsController(
        IPaymentService paymentService,
        IUnitOfWork unit,
        ILogger<PaymentsController> logger,
        IConfiguration config,
        IHubContext<NotificationHub> hubContext)
    {
        _paymentService = paymentService;
        _unit = unit;
        _logger = logger;
        _whSecret = config["StripeSettings:WhSecret"]!;
        _hubContext = hubContext; // assign it
    }

    [Authorize]
    [HttpPost("{cartId}")]
    public async Task<ActionResult<string>> CreateOrUpdatePaymentIntent(string cartId)
    {
        var cart = await _paymentService.CreateOrUpdatePaymentIntent(cartId);

        if (cart == null)
            return BadRequest(new ProblemDetails { Title = "Problem creating payment intent" });

        return Ok(cart);
    }

    [HttpGet("delivery-methods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
        var methods = await _unit.Repository<DeliveryMethod>().ListAllAsync();
        return Ok(methods);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = ConstructStripeEvent(json);

            // Only handle payment_intent.succeeded events
        if (stripeEvent?.Type == "payment_intent.succeeded")
        {
          var intent = stripeEvent.Data.Object as PaymentIntent;
        if (intent != null)
         {
        await HandlePaymentIntentSucceeded(intent);
         }
    }      // Respond 200 OK to all other events
            return Ok();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe webhook error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Stripe webhook error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in webhook");
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error occurred");
        }
    }

    private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
    {
        if (intent.Status != "succeeded") return;

        var spec = new OrderSpecification(intent.Id, true);
        var order = await _unit.Repository<OrderEntity>().GetEntityWithSpec(spec);

        if (order == null)
        {
            _logger.LogWarning("Order not found for PaymentIntent {PaymentIntentId}", intent.Id);
            return; // Do not throw — just log
        }

        if ((long)order.GetTotal() * 100 != intent.Amount)
            order.Status = OrderStatus.PaymentMisMatch;
        else
            order.Status = OrderStatus.PaymentReceived;

        await _unit.Complete();

        var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);
        
        if (!string.IsNullOrEmpty(connectionId))
        {
          await _hubContext.Clients.Client(connectionId).SendAsync("OrderCompleteNotification", order.ToDto());          
        }
    }

    private Event? ConstructStripeEvent(string json)
    {
        try
        {
            return EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _whSecret
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to construct Stripe event");
            return null; // returns null for invalid signature (avoids 500)
        }
    }
}