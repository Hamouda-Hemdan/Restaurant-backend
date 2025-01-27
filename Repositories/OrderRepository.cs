using Microsoft.EntityFrameworkCore;
using resturant1.Data;
using resturant1.Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace resturant1.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetUserOrdersAsync(string email)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.User.UserName == email)
                .ToListAsync();

            return orders;
        }
    }
}
