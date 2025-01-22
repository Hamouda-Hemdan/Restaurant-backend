using System.ComponentModel.DataAnnotations;

public class Basket
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public ICollection<BasketItem> Items { get; set; }


}
