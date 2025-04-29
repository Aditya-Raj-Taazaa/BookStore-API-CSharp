using Test_API.Models;

namespace Test_API.Data
{
    public class DataSeeder
    {
        private readonly BookdbContext _context;

        public DataSeeder(BookdbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            
            if (_context.Authors.Any() || _context.Books.Any())
            {
                Console.WriteLine("Database already seeded");
                return;
            }

            
            var authors = new[]
            {
            new Author
            {
                Name = "J.K Rowling",
                Bio = "Fantasy Genre Writer from England"
            }, 
        };

            
            await _context.Authors.AddRangeAsync(authors);
            await _context.SaveChangesAsync();

            
            var books = new[]
            {
            new Book
            {
                Title = "Harry Potter and The Chamber of Secrets",
                Price = 29,
                AuthorId = authors[0].Id
            },
            new Book
            {
                Title = "Harry Potter and The Deathly Hallows",
                Price = 24,
                AuthorId = authors[0].Id
            },
            new Book
            {
                Title = "Harry Potter and The Half Bood Prince",
                Price = 24,
                AuthorId = authors[0].Id
            }
        };

            
            await _context.Books.AddRangeAsync(books);
            await _context.SaveChangesAsync();

            Console.WriteLine("Database seeded successfully");
        }
    }
}