using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Test_API.Domains;
using Test_API.Interfaces;
using Test_API.DTO;

namespace Test_API.Controllers
{
    [ApiController]
    [Route("api/Authors")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(IAuthorService authorService, ILogger<AuthorController> logger)
        {
            _authorService = authorService;
            _logger = logger;
        }

        [HttpPost("GetAuthors")]
        public async Task<IActionResult> Post([FromBody] AuthorFilterDTO filters)
        {
            try
            {
                if (filters.Page <= 0 || filters.PageSize <= 0)
                    return BadRequest("Page and pageSize must be greater than 0.");

                var authors = await _authorService.ListAsync(filters);
                var totalAuthors = await _authorService.CountAsync(filters.Name, filters.Bio);

                return Ok(new
                {
                    TotalCount = totalAuthors,
                    Page = filters.Page,
                    PageSize = filters.PageSize,
                    Data = authors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching authors.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var author = await _authorService.FindById(id);
                if (author == null)
                {
                    return NotFound($"The author with ID {id} was not found.");
                }

                return Ok(author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the author with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("createAuthor")]
        public async Task<IActionResult> Post(AuthorDTO authorDTO)
        {
            try
            {
                var result = await _authorService.Post(authorDTO);
                return CreatedAtAction(nameof(GetById), new { id = result.Value?.Id }, result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an author.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, AuthorDTO authorDTO)
        {
            try
            {
                var result = await _authorService.UpdateAuthor(id, authorDTO);

                if (result.Result is BadRequestResult)
                {
                    return BadRequest("The provided ID does not match the author ID.");
                }
                if (result.Result is NotFoundResult)
                {
                    return NotFound("The author with the specified ID was not found.");
                }
                if (result.Result is ConflictResult)
                {
                    return Conflict("A concurrency issue occurred while updating the author.");
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the author.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _authorService.DeleteAuthor(id);

                if (result is NotFoundResult)
                {
                    return NotFound($"The author with ID {id} was not found.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the author.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> CountAuthors()
        {
            try
            {
                var count = await _authorService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting authors.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}