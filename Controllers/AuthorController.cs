using Microsoft.AspNetCore.Mvc;
using Test_API.Interfaces;
using Test_API.Models;
using Test_API.Models.DTOs;

namespace Test_API.Controllers
{
    [ApiController]
    [Route("api/Authors")]
    public class AuthorController : ControllerBase
    {
        private readonly IGenericService<Author, GetAuthorDTO, CreateAuthorDTO, UpdateAuthorDTO> _authorService;

        public AuthorController(IGenericService<Author, GetAuthorDTO, CreateAuthorDTO, UpdateAuthorDTO> authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1, int pageSize = 10)
        {
            var authors = await _authorService.ListAsync(page, pageSize);
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author.Result is NotFoundResult)
            {
                return NotFound($"The author with ID {id} was not found.");
            }

            return Ok(author.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateAuthorDTO createAuthorDTO)
        {
            var result = await _authorService.CreateAsync(createAuthorDTO);
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdateAuthorDTO updateAuthorDTO)
        {
            var result = await _authorService.UpdateAsync(id, updateAuthorDTO);
            if (result.Result is NotFoundResult)
            {
                return NotFound($"The author with ID {id} was not found.");
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _authorService.DeleteAsync(id);
            if (result is NotFoundResult)
            {
                return NotFound($"The author with ID {id} was not found.");
            }

            return result;
        }
    }
}