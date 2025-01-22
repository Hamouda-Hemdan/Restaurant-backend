namespace resturant1.Models.DTOs
{
    public class BasketItemDTO
    {
        public Guid DishId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Image { get; set; }
    }

    public class BasketDTO
    {
        public Guid Id { get; set; }
        public ICollection<BasketItemDTO> Items { get; set; }
    }
}
