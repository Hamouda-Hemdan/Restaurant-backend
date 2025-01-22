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


}
