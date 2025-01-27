using System;
using System.Collections.Generic;
using resturant1.Models.Entities;
using resturant1.Models.Enums; // Ensure this import for OrderItem

namespace resturant1.Models.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime DeliveryTime { get; set; }
        public DateTime OrderTime { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Price { get; set; }

        // Navigation property
        public User User { get; set; }
        public Guid UserId { get; set; }

        // Add a collection of OrderItems for the one-to-many relationship
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public string Address { get; internal set; }
        public List<BasketItem> Items { get; internal set; }
    }
}
