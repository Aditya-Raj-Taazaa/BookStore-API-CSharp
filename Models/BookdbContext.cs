using Microsoft.EntityFrameworkCore;

namespace Test_API.Models
{
    public class BookdbContext : DbContext
    {
        public BookdbContext(DbContextOptions<BookdbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author) // A Book has one Author
                .WithMany(a => a.Books) // An Author has many Books
                .HasForeignKey(b => b.AuthorId) // F.key
                .IsRequired();
        }  
    }
}
