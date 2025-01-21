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
   protected override void OnModelCreating(ModelBuilder modelBuilder)
     {
     base.OnModelCreating(modelBuilder);

     modelBuilder.Entity<User>()
         .Property(u => u.Id)
         .HasConversion(
             v => v.ToString(),
             v => Guid.Parse(v)
         );
         }

    }
}
