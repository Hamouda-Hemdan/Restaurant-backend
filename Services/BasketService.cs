using Microsoft.EntityFrameworkCore;
using resturant1.Data;
using resturant1.Repositories;

public class BasketService
{
    private readonly IBasketRepository _basketRepository;
    private readonly ApplicationDbContext _context; 

    public BasketService(IBasketRepository basketRepository, ApplicationDbContext context)
    {
        _basketRepository = basketRepository;
        _context = context;
    }

    // Get the user's basket by email
    public async Task<Basket> GetBasketAsync(string email)
    {
        return await _basketRepository.GetBasketByEmailAsync(email);
    }

    // Add a dish to the basket
    public async Task<(bool Success, string Message)> AddDishToBasketAsync(string email, Guid dishId)
    {
        var basket = await _basketRepository.GetBasketByEmailAsync(email);
    
        if (basket == null)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return (false, "User not found.");
            }
    
            basket = new Basket
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Items = new List<BasketItem>()
            };
    
            _context.Baskets.Add(basket);
            await _context.SaveChangesAsync(); // Ensure the basket is saved first
        }
    
        // Fetch the dish by ID
        var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
        if (dish == null)
        {
            return (false, "Dish not found.");
        }
    
        // Check if the dish is already in the basket
        var existingItem = basket.Items.FirstOrDefault(item => item.DishId == dishId);
        if (existingItem != null)
        {
            existingItem.Amount++;
            existingItem.TotalPrice = existingItem.Amount * existingItem.Price;
    
            // Explicitly mark this item as modified so that EF tracks it
            _context.Entry(existingItem).State = EntityState.Modified;
        }
        else
        {
            // Add the new dish to the basket
            var newItem = new BasketItem
            {
                Id = Guid.NewGuid(),
                Name = dish.Name,
                Price = (decimal)dish.Price,
                Amount = 1,
                TotalPrice = (decimal)dish.Price,
                BasketId = basket.Id,
                DishId = dish.Id,
                Image = dish.Image ?? "default-image-url"
            };
    
            basket.Items.Add(newItem);
    
            // Mark the new item as added
            _context.Entry(newItem).State = EntityState.Added;
        }
    
        try
        {
            // Save changes after marking the entity state
            await _context.SaveChangesAsync();
            return (true, "Dish added to basket successfully.");
        }
        catch (DbUpdateConcurrencyException)
        {
            return (false, "Concurrency error occurred while adding dish.");
        }
    }
    // Remove a dish from the basket
public async Task<(bool Success, string Message)> RemoveDishFromBasketAsync(string email, Guid dishId, bool inc)
{
    var basket = await _basketRepository.GetBasketByEmailAsync(email);
    if (basket == null)
    {
        return (false, "Basket not found.");
    }

    var item = basket.Items.FirstOrDefault(i => i.DishId == dishId);
    if (item == null)
    {
        return (false, "Dish not found in basket.");
    }

    if (inc)
    {
        // Decrease the amount
        item.Amount--;

        if (item.Amount <= 0)
        {
            // Remove item if amount becomes 0
            basket.Items.Remove(item);
        }
        else
        {
            // Update the total price
            item.TotalPrice = item.Amount * item.Price;
            _context.Entry(item).State = EntityState.Modified;
        }
    }
    else
    {
        // Completely remove the item
        basket.Items.Remove(item);
    }

    try
    {
        await _context.SaveChangesAsync();
        return (true, "Dish removed from basket.");
    }
    catch (Exception ex)
    {
        return (false, $"Error occurred: {ex.Message}");
    }
}


}
