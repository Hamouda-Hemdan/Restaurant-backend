using Microsoft.EntityFrameworkCore;
using resturant1.Data;
using resturant1.Models.Dto;
using resturant1.Models.DTOs;
using resturant1.Models.Entities;
using resturant1.Models.Enums;
using resturant1.Repositories;

namespace resturant1.Services
{
    public class DishService
    {
        private readonly IDishRepository _dishRepository;
        private readonly IRatingRepository _ratingRepository;
        private readonly ApplicationDbContext _context;

        public DishService(IDishRepository dishRepository, IRatingRepository ratingRepository, ApplicationDbContext context)
        {
            _dishRepository = dishRepository;
            _ratingRepository = ratingRepository;
            _context = context;
        }

        public async Task<PagedDishResponse> GetFilteredDishesAsync(
            List<string>? categories = null,
            bool? vegetarian = false,
            string? sorting = null,
            int page = 1,
            int pageSize = 5)
        {
            var dishes = await _dishRepository.GetAllDishesAsync() ?? new List<Dish>();

            if (categories != null && categories.Any())
            {
                dishes = dishes.Where(d => categories.Contains(d.Category.ToString())).ToList();
            }

            if (vegetarian.HasValue)
            {
                dishes = dishes.Where(d => d.Vegetarian == vegetarian.Value).ToList();
            }

            dishes = sorting switch
            {
                "NameAsc" => dishes.OrderBy(d => d.Name).ToList(),
                "NameDesc" => dishes.OrderByDescending(d => d.Name).ToList(),
                "PriceAsc" => dishes.OrderBy(d => d.Price).ToList(),
                "PriceDesc" => dishes.OrderByDescending(d => d.Price).ToList(),
                "RatingAsc" => dishes.OrderBy(d => d.Ratings.Any() ? d.Ratings.Average(r => r.Value) : 0).ToList(),
                "RatingDesc" => dishes.OrderByDescending(d => d.Ratings.Any() ? d.Ratings.Average(r => r.Value) : 0).ToList(),
                _ => dishes.OrderBy(d => d.Name).ToList(),
            };

            var totalCount = dishes.Count();
            dishes = dishes.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var pagedResponse = new PagedDishResponse
            {
                Dishes = dishes.Select(d => new DishDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Price = d.Price,
                    Rating = d.Ratings.Any() ? d.Ratings.Average(r => r.Value) : 0,
                    Description = d.Description,
                    Image = d.Image,
                    Vegetarian = d.Vegetarian,
                    Category = d.Category.ToString(),
                }).ToList(),
                Pagination = new PaginationMetadata
                {
                    Size = pageSize,
                    Count = totalCount,
                    Current = page
                }
            };

            return pagedResponse;
        }

        public async Task<DishDto> GetDishByIdAsync(Guid id)
        {
            var dish = await _dishRepository.GetDishByIdAsync(id);
            if (dish == null) return null;

            return new DishDto
            {
                Id = dish.Id,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                Image = dish.Image,
                Vegetarian = dish.Vegetarian,
                Rating = dish.Ratings.Any() ? dish.Ratings.Average(r => r.Value) : 0,
                Category = dish.Category.ToString()
            };
        }
        public async Task<bool> UserHasOrderedDishByEmailAsync(Guid dishId, string userEmail)
        {
            // Find the user by their email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Check if the user has completed an order containing the specified dish
            var hasOrderedDish = await _context.Orders
                .AnyAsync(o => o.UserId == user.Id &&
                               o.OrderItems.Any(oi => oi.DishId == dishId) &&
                               o.Status == OrderStatus.Completed);

            return hasOrderedDish;
        }

        

      


    }
}
