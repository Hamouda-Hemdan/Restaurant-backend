using resturant1.Models.Entities;
using System.ComponentModel.DataAnnotations;

public class BasketItem
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Amount { get; set; }
    public decimal TotalPrice { get; set; }
    public string Image { get; set; }

    public Guid BasketId { get; set; }
    public Basket Basket { get; set; }

    public Guid DishId { get; set; }
    public Dish Dish { get; set; }

}
