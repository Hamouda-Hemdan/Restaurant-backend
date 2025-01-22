using Microsoft.EntityFrameworkCore;
using resturant1.Data;
using resturant1.Models.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace resturant1.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly ApplicationDbContext _context;

        public BasketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Basket> GetBasketByEmailAsync(string email)
        {
            return await _context.Baskets
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.User.UserName == email);
        }

        public async Task ClearBasketAsync(Basket basket)
        {
            _context.BasketItems.RemoveRange(basket.Items);
            await _context.SaveChangesAsync();
        }
    }
}
