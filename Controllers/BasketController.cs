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
    
      // Add a dish to the basket
  [HttpPost("dish/{dishId}")]
  [Authorize]
  public async Task<IActionResult> AddDishToBasket(Guid dishId)
  {
      var email = User.FindFirstValue(ClaimTypes.Email);
      if (string.IsNullOrEmpty(email))
      {
          return Unauthorized("Invalid token or email not found.");
      }

      var result = await _basketService.AddDishToBasketAsync(email, dishId);
      if (!result.Success)
      {
          return BadRequest(result.Message);
      }

      return Ok(result.Message);
  }
  // Remove a dish from the basket
[HttpDelete("dish/{dishId}")]
[Authorize]
public async Task<IActionResult> RemoveDishFromBasket(Guid dishId, [FromQuery] bool inc = false)
{
    var email = User.FindFirstValue(ClaimTypes.Email);
    if (string.IsNullOrEmpty(email))
    {
        return Unauthorized("Invalid token or email not found.");
    }

    var result = await _basketService.RemoveDishFromBasketAsync(email, dishId, inc);
    if (!result.Success)
    {
        return BadRequest(result.Message);
    }

    return Ok(result.Message);
}

}
