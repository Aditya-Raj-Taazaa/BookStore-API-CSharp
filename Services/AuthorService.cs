
using Test_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Test_API.Interfaces;

namespace Test_API.Services
{
    public class AuthorService(BookdbContext context, FormatterService formatterService) : IAuthorService
    {
        private readonly BookdbContext _context = context;
        private readonly FormatterService _formatterService = formatterService;

        public async Task<int> CountAsync()
        {
            return await _context.Authors.CountAsync();
        }

        public async Task<IEnumerable<Author>> ListAsync(int page, int pageSize)
        {
            return await _context.Authors
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<ActionResult<Author>> Post(Author Author)
        {
            Author.Bio = _formatterService.BioFormat(Author.Bio);
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
                await _context.SaveChangesAsync();
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
