using resturant1.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace resturant1.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetUserOrdersAsync(string email);
        Task<Order> CreateOrderAsync(Order order);
    }
}
