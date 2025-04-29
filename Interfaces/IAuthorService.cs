using Microsoft.AspNetCore.Mvc;
using Test_API.Models;
using Test_API.Models.DTOs;

namespace Test_API.Interfaces
{
    public interface IAuthorService
    {
        Task<int> CountAsync(string? name = null, string? bio = null);
        Task<IEnumerable<GetAuthorDTO>> ListAsync(int page, int pageSize, string? name = null, string? bio = null);
        Task<ActionResult<AuthorDTO>> Post(CreateAuthorDTO createAuthorDTO);
        Task<ActionResult<AuthorDTO>> UpdateAuthor(int id, UpdateAuthorDTO updateAuthorDTO);
        Task<IActionResult> DeleteAuthor(int id);
        Task<GetAuthorDTO?> FindById(int id);
    }
}