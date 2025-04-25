using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test_API.Models;
using Test_API.ActionFilters;
using Test_API.Services;

namespace Test_API.Controllers
{
    [ApiController]
    [Route("api/Books")]
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> _logger;
        private readonly BookService _bookService;

        public BookController(ILogger<BookController> logger, BookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        [ExecutionTimeFilter]
        [HttpGet(Name = "GetBookDetails")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var books = await _bookService.ListAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching book details.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            try
            {
                var book = await _bookService.FindByIdAsync(id);
                if (book == null)
                {
                    return NotFound($"The book with ID {id} was not found.");
                }
                return Ok(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the book with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Book>> Post(Book book)
        {
            try
            {
                var result = await _bookService.CreateAsync(book);
                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a book.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Book book)
        {
            try
            {
                var result = await _bookService.UpdateBookAsync(id, book);

                if (result.Result is BadRequestResult)
                {
                    return BadRequest("The provided ID does not match the book ID.");
                }
                if (result.Result is NotFoundResult)
                {
                    return NotFound("The book with the specified ID was not found.");
                }
                if (result.Result is ConflictResult)
                {
                    return Conflict("A concurrency issue occurred while updating the book.");
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the book.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _bookService.DeleteBookAsync(id);

                if (result is NotFoundResult)
                {
                    return NotFound($"The book with ID {id} was not found.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the book.");
                return StatusCode(500, "Internal server error");
            }
        }

        [ExecutionTimeFilter]
        [HttpGet("count")]
        public async Task<IActionResult> CountBooks()
        {
            try
            {
                var count = await _bookService.ListAsync();
                return Ok(count.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting books.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}