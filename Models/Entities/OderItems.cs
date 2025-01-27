using resturant1.Models.Entities;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; }  // Navigation property to Order
    public Guid DishId { get; set; }
    public Dish Dish { get; set; }  // Navigation property to Dish
    public int Amount { get; set; }
    public decimal TotalPrice { get; set; }
}
