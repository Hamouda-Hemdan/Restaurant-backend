using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using resturant1.Models.DTOs.Order;
using resturant1.Services;
using System.Security.Claims;
using System;
using Swashbuckle.AspNetCore.Annotations;

namespace resturant1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

     

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO orderDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("Invalid token or email not found.");
            }

            var result = await _orderService.CreateOrderAsync(email, orderDto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpGet]
     [Authorize]
     public async Task<IActionResult> GetBasket()
     {
         var email = User.FindFirstValue(ClaimTypes.Email);
         if (string.IsNullOrEmpty(email))
         {
             return Unauthorized("Invalid token or email not found.");
         }
    
         var basket = await _basketService.GetBasketAsync(email);
         if (basket == null)
         {
             return Ok(new BasketDTO { Id = Guid.Empty, Items = new List<BasketItemDTO>() });
         }
    
         // Map entity to DTO
         var basketDto = new BasketDTO
         {
             Id = basket.Id,
             Items = basket.Items.Select(item => new BasketItemDTO
             {
                 DishId = item.DishId,
                 Name = item.Name,
                 Price = item.Price,
                 Amount = item.Amount,
                 TotalPrice = item.TotalPrice,
                 Image = item.Image
             }).ToList()
         };
    
         return Ok(basketDto);
     }


      


    }
}
