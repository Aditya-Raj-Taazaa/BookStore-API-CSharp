using Microsoft.AspNetCore.Mvc;
using Test_API.Models;
using Test_API.Services;

namespace Test_API.Controllers
{
    [ApiController]
    [Route("api/Authors")]
    public class AuthorController : ControllerBase
    {
        private readonly AuthorService _authorService;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(AuthorService authorService, ILogger<AuthorController> logger)
        {
            _authorService = authorService;
            _logger = logger;
        }

        [HttpGet(Name = "GetAuthors")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var authors = await _authorService.ListAsync();
                return Ok(authors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching authors.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(Author author)
        {
            try
            {
                var result = await _authorService.Post(author);
                return CreatedAtAction(nameof(GetById), new { id = author.Id }, result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an author.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Author author)
        {
            try
            {
                var result = await _authorService.UpdateAuthor(id, author);

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

        [HttpGet("count")]
        public async Task<IActionResult> Count_Authors()
        {
            try
            {
                var count = await _authorService.ListAsync();
                return Ok(count.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting authors.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}