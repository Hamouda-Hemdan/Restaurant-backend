using resturant1.Models.Entities;

public class Rating
{
    public Guid Id { get; set; }  // Primary key
    public Guid DishId { get; set; }  // Foreign key to Dish
    public int Value { get; set; }  // Rating value (e.g., 1 to 10)

    // Foreign key to User
    public Guid UserId { get; set; }

    // Navigation property to the Dish entity
    public Dish Dish { get; set; }

    // Navigation property to the User entity
    public User User { get; set; }  // This will link Rating to User
}
