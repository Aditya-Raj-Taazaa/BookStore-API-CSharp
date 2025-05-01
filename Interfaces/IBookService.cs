using Microsoft.AspNetCore.Mvc;
using Test_API.Domains;
using Test_API.DTO;

namespace Test_API.Interfaces
{
    public interface IBookService
    {
        Task<int> CountAsync(string? title = null, int? price = null);
        Task<IEnumerable<GetBookDTO>> ListAsync(BookFilterDTO filter);
        Task<ActionResult<BookDTO>> Post(BookDTO bookDTO);
        Task<ActionResult<BookDTO>> UpdateBook(int id, BookDTO bookDTO);
        Task<IActionResult> DeleteBook(int id);
        Task<GetBookDTO?> FindById(int id);
    }
}