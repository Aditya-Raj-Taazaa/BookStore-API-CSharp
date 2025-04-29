using Microsoft.AspNetCore.Mvc;
using Test_API.Models;

namespace Test_API.Interfaces
{
    public interface IAuthorService
    {
        Task<int> CountAsync();
        Task<IEnumerable<Author>> ListAsync(int page, int pageSize);
        Task<ActionResult<Author>> Post(Author author);
        Task<ActionResult<Author>> UpdateAuthor(int id, Author author);
        Task<IActionResult> DeleteAuthor(int id);
        Task<Author?> FindById(int id);
    }
}