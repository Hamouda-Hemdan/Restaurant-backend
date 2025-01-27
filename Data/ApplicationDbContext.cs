using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using resturant1.Models.Entities;
using resturant1.Models;

namespace resturant1.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasConversion(
                    v => v.ToString(),
                    v => Guid.Parse(v)
                );

            modelBuilder.Entity<BasketItem>()
                .Property(bi => bi.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<BasketItem>()
                .Property(bi => bi.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Basket>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<BasketItem>()
                .HasOne(bi => bi.Dish)
                .WithMany()
                .HasForeignKey(bi => bi.DishId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<OrderItem>()
                 .HasOne(oi => oi.Order)
                 .WithMany(o => o.OrderItems)
                 .HasForeignKey(oi => oi.OrderId)
                 .OnDelete(DeleteBehavior.Cascade); // Optional: Defines the delete behavior (Cascade deletes OrderItems when Order is deleted)

            // Configure the relationship between OrderItem and Dish
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Dish)
                .WithMany()
                .HasForeignKey(oi => oi.DishId);

            modelBuilder.Entity<Rating>()
            .HasOne(r => r.User)  // Rating has one User
            .WithMany()  // User can have many Ratings
            .HasForeignKey(r => r.UserId);

        }
    }
}
