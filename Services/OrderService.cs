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

    public async Task<(bool Success, string Message)> CreateOrderAsync(string email, CreateOrderDTO orderDto)
    {
        var basket = await _basketRepository.GetBasketByEmailAsync(email);
        if (basket == null || !basket.Items.Any())
        {
            return (false, "No items in the basket to create an order.");
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = basket.UserId,
            OrderTime = DateTime.UtcNow,
            DeliveryTime = orderDto.DeliveryTime,
            Address = orderDto.Address,
            Status = OrderStatus.InProcess,
            Price = basket.Items.Sum(item => item.TotalPrice),
            Items = basket.Items.ToList()
        };

        _context.Orders.Add(order);

        // Create OrderItems based on the BasketItems
        foreach (var basketItem in basket.Items)
        {
            var orderItem = new OrderItem
            {
                DishId = basketItem.DishId,
                Amount = basketItem.Amount,
                TotalPrice = basketItem.TotalPrice,
                OrderId = order.Id
            };

            _context.OrderItems.Add(orderItem);
        }

        await _context.SaveChangesAsync();

        // Clear the basket after creating the order
        _context.BasketItems.RemoveRange(basket.Items);
        await _context.SaveChangesAsync();

        return (true, "Order created successfully.");
    }

    public async Task<(bool Success, string Message)> UpdateOrderStatusAsync(Guid orderId, string email)
    {
        var order = await _context.Orders
            .Where(o => o.Id == orderId && o.User.UserName == email)
            .FirstOrDefaultAsync();

        if (order == null)
        {
            return (false, "Order not found for this user.");
        }

        if (order.Status == OrderStatus.Completed)
        {
            return (false, "Order is already completed.");
        }

        // Change the status to 'Completed'
        order.Status = OrderStatus.Completed;
        await _context.SaveChangesAsync();

        return (true, "Order status updated to Completed.");
    }

   



}
