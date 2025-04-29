using Microsoft.AspNetCore.Mvc;
using Test_API.Models;

namespace Test_API.Interfaces
{
    public interface IBookService
    {
        Task<List<Book>> ListAsync(int page, int pageSize);
        Task<ActionResult<Book>> Post(Book book);
        Task<ActionResult<Book>> UpdateBook(int id, Book book);
        Task<IActionResult> DeleteBook(int id);
        Task<Book?> FindById(int id);
    }
}