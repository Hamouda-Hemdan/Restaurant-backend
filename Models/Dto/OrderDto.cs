namespace resturant1.Models.DTOs.Order
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public DateTime OrderTime { get; set; }
        public DateTime DeliveryTime { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public string Address { get; set; }
        public List<OrderItemDTO> Dishes { get; set; }  // List of OrderItems
    }
}
