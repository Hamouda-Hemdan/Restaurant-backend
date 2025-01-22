using resturant1.Models.Entities;
using System.Threading.Tasks;

namespace resturant1.Repositories
{
    public interface IBasketRepository
    {
        Task<Basket> GetBasketByEmailAsync(string email);
        Task ClearBasketAsync(Basket basket);
    }
}
