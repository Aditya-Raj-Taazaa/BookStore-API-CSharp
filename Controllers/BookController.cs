using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test_API.Domains;
using Test_API.DTO;
using Test_API.ActionFilters;
using Test_API.Services;
using Test_API.Interfaces;
using AutoMapper;

namespace Test_API.Controllers
{
    [ApiController]
    [Route("api/Books")]
    public class BookController : ControllerBase
    {
        private readonly BookdbContext _context; 
        private readonly ILogger<BookController> _logger;
        private readonly IAppInfoService _appInfoService;
        private readonly IBookService _bookService;

        public BookController(BookdbContext context, ILogger<BookController> logger, IAppInfoService appInfoService, IBookService bookService)
        {
            _context = context;
            _logger = logger;
            _appInfoService = appInfoService;
            _bookService = bookService;
        }
       
        [ExecutionTimeFilter]
        [HttpPost("GetBookDetails")]
        public async Task<IActionResult> Post([FromBody] BookFilterDTO filter)
        {
            if (filter.Page <= 0 || filter.PageSize <= 0)
                return BadRequest("Page and PageSize must be positive integers.");

            try
            {
                var books = await _bookService.ListAsync(filter);
                var totalBooks = await _bookService.CountAsync(filter.Title, filter.Price);
                return Ok(new
                {
                    TotalCount = totalBooks,
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    Data = books
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching books.");
                return StatusCode(500, "Internal server error");
            }
        }

        
        [HttpPost]
        public async Task<IActionResult> Post(BookDTO bookDTO)
        {
            try
            {
                var result = await _bookService.Post(bookDTO);

                if (result.Result is BadRequestObjectResult badRequest)
                {
                    return BadRequest(badRequest.Value);
                }

                return CreatedAtAction(nameof(GetBook), new { id = result.Value?.Id }, result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a book.");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, BookDTO bookDTO)
        {
            try
            {
                var result = await _bookService.UpdateBook(id, bookDTO);
                if (result.Result is BadRequestResult)
                    return BadRequest("The provided ID does not match the book ID.");
                if (result.Result is NotFoundResult)
                    return NotFound($"The book with ID {id} was not found.");
                if (result.Result is ConflictResult)
                    return Conflict("A concurrency issue occurred while updating the book.");

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
            try
            {
                var book = await _bookService.FindById(id);
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

        [ExecutionTimeFilter]
        [HttpPost("count")]
        public async Task<IActionResult> CountBooks([FromBody] BookFilterDTO filter)
        {
            try
            {
                var totalBooks = await _bookService.CountAsync(filter.Title, filter.Price);
                return Ok(totalBooks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting books.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
