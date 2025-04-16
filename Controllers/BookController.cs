using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test_API.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Test_API.ActionFilters;

namespace Test_API.Controllers
{
    [ApiController]
    [Route("api/Books")]
    public class BookController : ControllerBase
    {
        private readonly BookdbContext _context; // Ensure the type matches the constructor parameter
        private readonly ILogger<BookController> _logger;

        public BookController(BookdbContext context, ILogger<BookController> logger) // Change parameter type to BookdbContext
        {
            _context = context;
            _logger = logger;
        }

        [ExecutionTimeFilter]
        [HttpGet(Name = "GetBookDetails")]
        public async Task<IEnumerable<Book>> Get()
        {
            return await _context.Books.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Book>> Post(Book Book)
        {
            _context.Books.Add(Book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = Book.Id }, Book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Book Book)
        {
            try
            {
                if (id != Book.Id)
                {
                    return BadRequest();
                }

                _context.Entry(Book).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(Book);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var Book = await _context.Books.FindAsync(id);
                if (Book == null)
                {
                    return NotFound();
                }

                _context.Books.Remove(Book);
                await _context.SaveChangesAsync();

                return StatusCode(204);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting Book Details");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("v1/{id}")]
        public async Task<IActionResult> Get_By_Id(int id)
        {
            try
            {
                var Book = await _context.Books.FindAsync(id);
                if (Book == null)
                {
                    return NotFound();
                }
                return Ok(Book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting Book details with {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("v2/{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _context.Books
                .Where(b => b.Id == id)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Price,
                    b.AuthorId,
                    AuthorName = _context.Authors
                        .Where(a => a.Id == b.AuthorId)
                        .Select(a => a.Name)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (book == null) return NotFound();
            return Ok(book);
        }


        [HttpGet("count")]
        public async Task<IActionResult> Count_Books()
        {
            try
            {
                var count = await _context.Books.CountAsync();
                Console.WriteLine(count);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting Books.");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool BookExists(int id)
        {
            var Book = _context.Books.Find(id);
            return Book != null;
        }
    }
}
