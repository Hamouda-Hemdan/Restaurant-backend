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
        [ProducesResponseType(typeof(PagedDishResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [SwaggerOperation(Summary = "Get a list of dishes(menu)")]
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

        

       

        
    }
}
