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

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Git a list of orders")]
        public async Task<IActionResult> GetOrders()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("Invalid token or email not found.");
            }

            var orders = await _orderService.GetUserOrdersAsync(email);
            if (orders == null || !orders.Any())
            {
                return Ok(new List<OrderDTO>());
            }

            return Ok(orders);
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Creating the order from dishes in basket")]
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

     

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Get information about concrete order")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("Invalid token or email not found.");
            }

            var order = await _orderService.GetOrderByIdAsync(id, email);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            return Ok(order);
        }


    }
}
