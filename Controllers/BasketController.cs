using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using resturant1.Models.DTOs;
using resturant1.Models.Entities;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly BasketService _basketService;

    public BasketController(BasketService basketService)
    {
        _basketService = basketService;
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
