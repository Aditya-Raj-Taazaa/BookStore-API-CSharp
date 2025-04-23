
using Test_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Test_API.Services
{
    public class BookService(BookdbContext context)
    {
        private readonly BookdbContext _context = context;

        public async Task<IEnumerable<Book>> ListAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<ActionResult<Book>> Post(Book Book)
        {
            _context.Books.Add(Book);
            await _context.SaveChangesAsync();
            return new ActionResult<Book>(Book);
        }

        public async Task<ActionResult<Book>> UpdateBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return new BadRequestResult();
            }

            var existingBook = await FindById(id);
            if (existingBook == null)
            {
                return new NotFoundResult();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(); // Ensure this is awaited
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("Concurrency exception: " + ex.Message);
                return new ConflictResult();
            }

            return new ActionResult<Book>(book);
            
        }

        public async Task<IActionResult> DeleteBook(int id)
        {
            var bookToDelete = await FindById(id);
            if(bookToDelete == null)
            {
                return new NotFoundResult();
            }

            _context.Books.Remove(bookToDelete);
            await _context.SaveChangesAsync();
            return new OkObjectResult(bookToDelete);  
        }

        private async Task<bool> BookExists(int id)
        {
            return await _context.Books.AnyAsync(b => b.Id == id);
        }

        private async Task<Book?> FindById(int id)
{
    return await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
}
    }
}
