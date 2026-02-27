
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using core.Entities.OrderAggregate;
using core.Interface;
using API.DTOs;
using API.Extensions;
using core.Entities;
using core.Specifications;


namespace API.Controllers;


[Authorize]
public class OrdersController(ICartService cartService, IUnitOfWork unit): BaseApiController
{

[HttpPost]
public async Task<ActionResult<Order>> CreateOrder( CreateOrderDto orderDto)
    {
       var email = User.GetEmail();
       var cart = await cartService.GetCartAsync(orderDto.CartId);
       if (cart == null) return BadRequest("Problem with your cart");
       if (cart.PaymentIntentId == null) return BadRequest("No payment intent found for this cart");
       var items = new List<OrderItem>();
       foreach (var item in cart.Items)
       {
           var productItem = await unit.Repository<Product>().GetByIdAsync(item.ProductId);
           if (productItem == null) return BadRequest("Problem with the order");
           var itemOrdered = new ProductItemOrdered
           {
               ProductId = item.ProductId,
               ProductName = item.ProductName,
               PictureUrl = item.PictureUrl
           };
           var orderItem = new OrderItem
           {
               ItemOrdered = itemOrdered,
               Price = productItem.Price,
               Quantity = item.Quantity
           };
           items.Add(orderItem);
       }

         var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId);
         if (deliveryMethod == null) return BadRequest("Problem with the order");
         
         var order = new Order
         {
             OrderItems = items,
             DeliveryMethod = deliveryMethod,
             ShippingAddress = orderDto.ShippingAddress,
             Subtotal = items.Sum(x => x.Price * x.Quantity),
             PaymentSummary = orderDto.PaymentSummary,
             PaymentIntentId = cart.PaymentIntentId,
             BuyerEmail = email
         };
         unit.Repository<Order>().Add(order);
         
         if (await unit.Complete())
        {
          return order;  
        } 
            return BadRequest("Problem creating order");
    }
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser()
    {
        var spec = new OrderSpecification(User.GetEmail());
        var orders = await unit.Repository<Order>().ListAsync(spec);
        var OrdersToReturn = orders.Select(x => x.ToDto()).ToList();
        return Ok (OrdersToReturn);
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(int id)
    {
        var spec = new OrderSpecification(User.GetEmail(), id);
        var order = await unit.Repository<Order>().GetEntityWithSpec(spec);
        if (order == null) return NotFound();
        return order.ToDto();
    }
}
