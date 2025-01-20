using resturant1.Data;  // Ensure this is present

using Microsoft.EntityFrameworkCore;
using resturant1.Models.Entities;
using WebApplication1.Data;

namespace resturant1.Repositories
{
    public class DishRepository : IDishRepository
    {
        private readonly ApplicationDbContext _context;

        public DishRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Synchronous method (you can leave this or remove if not needed)
        public IEnumerable<Dish> GetAllDishes()
        {
            return _context.Dishes.Include(d => d.Ratings).ToList();
        }

        // Asynchronous method
        public async Task<IEnumerable<Dish>> GetAllDishesAsync()
        {
            return await _context.Dishes.Include(d => d.Ratings).ToListAsync(); // Asynchronous version
        }

        // Synchronous method (you can leave this or remove if not needed)
        public Dish GetDishById(Guid id)
        {
            return _context.Dishes.Include(d => d.Ratings).FirstOrDefault(d => d.Id == id);
        }

        // Asynchronous method
        public async Task<Dish> GetDishByIdAsync(Guid id)
        {
            return await _context.Dishes.Include(d => d.Ratings).FirstOrDefaultAsync(d => d.Id == id); // Asynchronous version
        }

        // Synchronous method (you can leave this or remove if not needed)
        public bool RatingExists(Guid dishId)
        {
            return _context.Ratings.Any(r => r.DishId == dishId);
        }

        // Asynchronous method
        public async Task<bool> RatingExistsAsync(Guid dishId)
        {
            return await _context.Ratings.AnyAsync(r => r.DishId == dishId); // Asynchronous version
        }

        // Synchronous method (you can leave this or remove if not needed)
        public void AddRating(Rating rating)
        {
            _context.Ratings.Add(rating);
            _context.SaveChanges();
        }

        // Asynchronous method
        public async Task AddRatingAsync(Rating rating)
        {
            await _context.Ratings.AddAsync(rating); // Asynchronous version
            await _context.SaveChangesAsync(); // Asynchronous version
        }
    }
}
