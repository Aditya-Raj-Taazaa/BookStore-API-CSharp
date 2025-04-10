using Microsoft.EntityFrameworkCore;
using Test_API.Models;

namespace Test_API.Models
{
    public class CarContext : DbContext
    {
        public CarContext(DbContextOptions<CarContext> options)
        : base(options)
        {
        }

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