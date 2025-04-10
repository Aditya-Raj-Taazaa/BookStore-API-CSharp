using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Test_API.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }
        public required string Make { get; set; }
        public required string Model { get; set; }
        public int Year { get; set; }

        public int wheels { get; set; }

        public int UserId { get; set; } // F.Key

        public User user { get; set; }
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                        .HasMany<Car>()
                        .WithOne(c => c.user)
                        .HasForeignKey(c => c.UserId);
        }
    }
}
