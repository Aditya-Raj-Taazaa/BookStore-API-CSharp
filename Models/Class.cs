using Microsoft.EntityFrameworkCore;

namespace Test_API.Models
{
    public class CarContext : DbContext
    {
        public CarContext(DbContextOptions<CarContext> options)
            : base(options)
        {
        }
        public DbSet<Car> Cars { get; set; }
    }
}
