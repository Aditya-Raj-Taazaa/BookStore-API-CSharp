using Microsoft.AspNetCore.Mvc;
using Test_API.Domains;
using Test_API.DTO;

namespace Test_API.Interfaces
{
    public interface IAuthorService
    {
        Task<int> CountAsync(string? name = null, string? bio = null);
        Task<IEnumerable<GetAuthorDTO>> ListAsync(AuthorFilterDTO filters);
        Task<ActionResult<AuthorDTO>> Post(AuthorDTO authorDTO);
        Task<ActionResult<AuthorDTO>> UpdateAuthor(int id, AuthorDTO authorDTO);
        Task<IActionResult> DeleteAuthor(int id);
        Task<GetAuthorDTO?> FindById(int id);
    }
}