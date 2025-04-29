using Microsoft.AspNetCore.Mvc;
using Test_API.Interfaces;
using Test_API.Models;
using Test_API.Models.DTOs;

namespace Test_API.Controllers
{
    [ApiController]
    [Route("api/Books")]
    public class BookController : ControllerBase
    {
        private readonly IGenericService<Book, GetBookDTO, CreateBookDTO, UpdateBookDTO> _bookService;

        public BookController(IGenericService<Book, GetBookDTO, CreateBookDTO, UpdateBookDTO> bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1, int pageSize = 10)
        {
            var books = await _bookService.ListAsync(page, pageSize);
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book.Result is NotFoundResult)
            {
                return NotFound($"The book with ID {id} was not found.");
            }

            return Ok(book.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateBookDTO createBookDTO)
        {
            var result = await _bookService.CreateAsync(createBookDTO);
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdateBookDTO updateBookDTO)
        {
            var result = await _bookService.UpdateAsync(id, updateBookDTO);
            if (result.Result is NotFoundResult)
            {
                return NotFound($"The book with ID {id} was not found.");
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookService.DeleteAsync(id);
            if (result is NotFoundResult)
            {
                return NotFound($"The book with ID {id} was not found.");
            }

            return result;
        }
    }
}