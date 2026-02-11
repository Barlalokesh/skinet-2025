using System;
using core.Entities;
using core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PaymentsController (IPaymentService paymentService, 
IGenericRepository<DeliveryMethod> dmRepo) : BaseApiController
{
    [Authorize]
    [HttpPost ("{cartId}")]
    public async Task<ActionResult<string>> CreateOrUpdatePaymentIntent(string cartId)
    {
        var cart  = await paymentService.CreateOrUpdatePaymentIntent(cartId);

        if (cart == null) return BadRequest(new ProblemDetails { Title = "Problem creating payment intent" });

        return Ok(cart );
    }
   [HttpGet("delivery-methods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
        return Ok(await dmRepo.ListAllAsync());

    }

}
