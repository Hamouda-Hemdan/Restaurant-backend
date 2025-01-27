using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using resturant1.Models.Dto;
using resturant1.Models.DTOs;
using resturant1.Models.Entities;
using resturant1.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace resturant1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DishController : ControllerBase
    {
        private readonly DishService _dishService;

        public DishController(DishService dishService)
        {
            _dishService = dishService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllDishes(
            [FromQuery] List<DishCategory>? categories = null,
            [FromQuery] bool? vegetarian = false,
            [FromQuery] SortingOptions? sorting = null,
            [FromQuery] int page = 1)
        {
            try
            {
                int pageSize = 5;

                var result = await _dishService.GetFilteredDishesAsync(
                    categories?.Select(c => c.ToString()).ToList(),
                    vegetarian,
                    sorting?.ToString(),
                    page,
                    pageSize
                );

                var pagedResponse = new PagedDishResponse
                {
                    Dishes = result.Dishes,
                    Pagination = new PaginationMetadata
                    {
                        Size = pageSize,
                        Count = result.TotalCount,
                        Current = page
                    }
                };

                return Ok(pagedResponse);
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Status = "500",
                    Message = "An error occurred while retrieving dishes."
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}")
        public async Task<IActionResult> GetDishById(Guid id)
        {
            try
            {
                var dish = await _dishService.GetDishByIdAsync(id);
                if (dish == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Status = "404",
                        Message = "Dish not found."
                    });
                }
                return Ok(dish);
            }
        }
    [Authorize]
 [HttpGet("{dishId}/rating/check")]
 public async Task<IActionResult> CanUserRateDish(Guid dishId)
 {
     try
     {
         var email = User.FindFirstValue(ClaimTypes.Email);
         if (string.IsNullOrEmpty(email))
         {
             return Unauthorized(new ErrorResponse
             {
                 Status = "401",
                 Message = "Email not found in token."
             });
         }

         var canRate = await _dishService.CanUserRateDishAsync(email, dishId);
         return Ok(canRate);
     }
     catch (Exception)
     {
         return StatusCode(500, new ErrorResponse
         {
             Status = "500",
             Message = "An error occurred while checking rating eligibility."
         });
     }
 }

 [Authorize]
 [HttpPost("{dishId}/rating")]
 public async Task<IActionResult> RateDish(Guid dishId, [FromBody] RatingDto ratingDto)
 {
     try
     {
         var email = User.FindFirstValue(ClaimTypes.Email);
         if (string.IsNullOrEmpty(email))
         {
             return Unauthorized(new ErrorResponse
             {
                 Status = "401",
                 Message = "Invalid token or email not found."
             });
         }

         var hasOrderedDish = await _dishService.UserHasOrderedDishByEmailAsync(dishId, email);
         if (!hasOrderedDish)
         {
             return BadRequest(new ErrorResponse
             {
                 Status = "400",
                 Message = "You can only rate dishes that you have ordered."
             });
         }

         await _dishService.AddOrUpdateRatingAsync(dishId, email, ratingDto);
         return Ok("Rating added or updated successfully.");
     }
     catch (Exception ex)
     {
         return StatusCode(500, new ErrorResponse
         {
             Status = "500",
             Message = $"An error occurred while adding or updating the rating: {ex.Message}"
         });
     }
 }
        
    }
}
