using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test_API.Models;
using Test_API.Models.DTOs;
using Test_API.ActionFilters;
using Test_API.Services;
using Azure;
using AutoMapper;

namespace Test_API.Controllers
{
    [ApiController]
    [Route("api/Books")]
    public class BookController : ControllerBase
    {
        private readonly BookdbContext _context; 
        private readonly ILogger<BookController> _logger;
        public readonly AppInfoService _appInfoService;
        public readonly BookService _bookService;
        public readonly IMapper _mapper;
        public BookController(BookdbContext context, ILogger<BookController> logger, AppInfoService appInfoService, BookService bookService, IMapper mapper) // Change parameter type to BookdbContext
        {
            _context = context;
            _logger = logger;
            _appInfoService = appInfoService;
            _bookService = bookService;
            _mapper = mapper;
        }
       
        [ExecutionTimeFilter]
        [HttpGet(Name = "GetBookDetails")]
        public async Task<IActionResult> Get([FromQuery] int page = 1, int pagesize = 1)
        {
            if (page <= 0 || pagesize <= 0)
                return BadRequest("Page and pagesize must be positive integers.");

            try
            {
                var books = await _bookService.ListAsync(page, pagesize);

                var bookDTOs = _mapper.Map<IEnumerable<GetBookDTO>>(books);

                return Ok(bookDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching books.");
                return StatusCode(500, "Internal server error");
            }
        }

        
        [HttpPost]
        public async Task<ActionResult<Book>> Post(Book book)
        {
            try{
                return await _bookService.Post(book);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"Error on Creating Book");
                return StatusCode(500,"Internal Server Error");
            }
            
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Book book)
        {
            var result = await _bookService.UpdateBook(id, book);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var Book = await _bookService.DeleteBook(id);

                if (Book is NotFoundResult)
                {
                    return NotFound($"The Book not Found By ID : {id}");
                }
                return Book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting Book Details");
                return StatusCode(500, "Internal server error");
            }
        }

        
        
        [ExecutionTimeFilter]
        [HttpGet("{id}")]
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

        [ExecutionTimeFilter]
        [HttpGet("count")]
        public async Task<IActionResult> CountBooks()
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
