using Microsoft.AspNetCore.Mvc;
using Test_API.Models;
using Test_API.Models.DTOs;

namespace Test_API.Interfaces
{
    public interface IBookService
    {
        Task<List<GetBookDTO>> ListAsync(int page, int pageSize );
        Task<ActionResult<BookDTO>> Post(CreateBookDTO createBookDTO);
        Task<ActionResult<BookDTO>> UpdateBook(int id, UpdateBookDTO updateBookDTO);
        Task<IActionResult> DeleteBook(int id);
        Task<Book?> FindById(int id);
    }
}