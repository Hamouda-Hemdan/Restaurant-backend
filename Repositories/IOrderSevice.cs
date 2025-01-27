using resturant1.Models.DTOs.Order;

public interface IOrderService
{
    Task<List<OrderDTO>> GetUserOrdersAsync(string email);
    Task<(bool Success, string Message)> CreateOrderAsync(string email, CreateOrderDTO orderDto);
    Task<(bool Success, string Message)> UpdateOrderStatusAsync(Guid orderId, string email);
    Task<OrderDTO> GetOrderByIdAsync(Guid orderId, string email);  // New method
}
