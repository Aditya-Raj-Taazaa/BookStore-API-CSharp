using Microsoft.AspNetCore.Mvc;
using Test_API.Models;
using Test_API.Models.DTOs;

namespace Test_API.Interfaces
{
    public interface IBookService
    {
        Task<int> CountAsync(string? title = null, int? price = null);
        Task<IEnumerable<GetBookDTO>> ListAsync(int page, int pageSize, string? title = null, int? price = null);
        Task<ActionResult<BookDTO>> Post(CreateBookDTO createBookDTO);
        Task<ActionResult<BookDTO>> UpdateBook(int id, UpdateBookDTO updateBookDTO);
        Task<IActionResult> DeleteBook(int id);
        Task<BookDTO?> FindById(int id);
    }
}