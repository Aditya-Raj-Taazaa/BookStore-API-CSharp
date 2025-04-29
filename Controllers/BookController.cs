using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test_API.Models;
using Test_API.Models.DTOs;
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
        public readonly AppInfoService _appInfoService;
        public readonly IBookService _bookService;
        public readonly IMapper _mapper;
        public BookController(BookdbContext context, ILogger<BookController> logger, AppInfoService appInfoService, IBookService bookService, IMapper mapper) // Change parameter type to BookdbContext
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
                var totalAuthors = books.Count();
                var bookDTOs = _mapper.Map<IEnumerable<GetBookDTO>>(books);
                
                return Ok(new
                {
                    TotalCount = totalAuthors,
                    Page = page,
                    PageSize = pagesize,
                    Data = bookDTOs
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching books.");
                return StatusCode(500, "Internal server error");
            }
        }

        
        [HttpPost]
        public async Task<ActionResult<Book>> Post(CreateBookDTO createBookDTO)
        {
            try
            {
                var book = _mapper.Map<Book>(createBookDTO);
                var result = await _bookService.Post(book);
                var bookDTO = _mapper.Map<BookDTO>(result.Value);
                return CreatedAtAction(nameof(GetBook),new {id = bookDTO.Id }, bookDTO);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"Error on Creating Book");
                return StatusCode(500,"Internal Server Error");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdateBookDTO updateBookDTO)
        {
            var existingBook = await _bookService.FindById(id);
            if(existingBook == null)
            {
                return NotFound($"Book by the Id {id} not Found");
            }
            _mapper.Map(updateBookDTO,existingBook);
            
            var result = await _bookService.UpdateBook(id, existingBook);

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
            try
            {
                var book = await _bookService.FindById(id);
                if (book == null)
                {
                    return NotFound($"The book with ID {id} was not found.");
                }

                var bookDTO = _mapper.Map<GetBookDTO>(book);
                return Ok(bookDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the book with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        [ExecutionTimeFilter]
        [HttpGet("count")]
        public async Task<IActionResult> CountBooks()
        {
            try
            {
                var count = await _bookService.ListAsync(1, int.MaxValue);
                return Ok(count.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting Books.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
