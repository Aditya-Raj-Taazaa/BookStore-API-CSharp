
using Test_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Test_API.Services
{
    public class AuthorService(BookdbContext context)
    {
        private readonly BookdbContext _context = context;

        public async Task<IEnumerable<Author>> ListAsync()
        {
            return await _context.Authors.ToListAsync();
        }

        public async Task<ActionResult<Author>> Post(Author Author)
        {
            _context.Authors.Add(Author);
            await _context.SaveChangesAsync();
            return new ActionResult<Author>(Author);
        }

        public async Task<ActionResult<Author>> UpdateAuthor(int id, Author author)
        {
            if (id != author.Id)
            {
                return new BadRequestResult();
            }

            var existingAuthor = await FindById(id);
            if (existingAuthor == null)
            {
                return new NotFoundResult();
            }

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(); // Ensure this is awaited
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("Concurrency exception: " + ex.Message);
                return new ConflictResult();
            }

            return new ActionResult<Author>(author);
            
        }

        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var AuthorToDelete = await FindById(id);
            if(AuthorToDelete == null)
            {
                return new NotFoundResult();
            }

            _context.Authors.Remove(AuthorToDelete);
            await _context.SaveChangesAsync();
            return new OkObjectResult(AuthorToDelete);  
        }

        private async Task<bool> AuthorExists(int id)
        {
            return await _context.Authors.AnyAsync(b => b.Id == id);
        }

        public async Task<Author?> FindById(int id)
        {
            return await _context.Authors.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
