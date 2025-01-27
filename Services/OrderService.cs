using resturant1.Data;
using resturant1.Models.DTOs.Order;
using resturant1.Models.DTOs;
using resturant1.Models.Entities;
using resturant1.Models.Enums;
using resturant1.Repositories;
using Microsoft.EntityFrameworkCore;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly IBasketRepository _basketRepository;

    public OrderService(ApplicationDbContext context, IBasketRepository basketRepository)
    {
        _context = context;
        _basketRepository = basketRepository;
    }

    public async Task<List<OrderDTO>> GetUserOrdersAsync(string email)
    {
        var orders = await _context.Orders
            .Where(o => o.User.UserName == email)
            .Select(o => new OrderDTO
            {
                Id = o.Id,
                OrderTime = o.OrderTime,
                DeliveryTime = o.DeliveryTime,
                Address = o.Address,
                Status = o.Status.ToString(),
                Price = o.Price,

            })
            .ToListAsync();

        return orders;
    }

   
}
