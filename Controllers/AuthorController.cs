using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Test_API.Models;
using Test_API.Services;
using Test_API.Interfaces;
using Test_API.Models.DTOs;

namespace Test_API.Controllers
{
    [ApiController]
    [Route("api/Authors")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly ILogger<AuthorController> _logger;
        private readonly IMapper _mapper;

        public AuthorController(IAuthorService authorService, ILogger<AuthorController> logger, IMapper mapper)
        {
            _authorService = authorService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetAuthors")]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest("Page and pageSize must be greater than 0.");
                }
                
                var authors = await _authorService.ListAsync(page, pageSize);
                var totalAuthors = await _authorService.CountAsync();
                var authorDTO = _mapper.Map<IEnumerable<GetAuthorDTO>>(authors);

                return Ok(new
                {
                    TotalCount = totalAuthors,
                    Page = page,
                    PageSize = pageSize,
                    Data = authorDTO
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching authors.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateAuthorDTO createAuthorDTO)
        {
            try
            {
                var author = _mapper.Map<Author>(createAuthorDTO);
                var result = await _authorService.Post(author);
                var authorDTO = _mapper.Map<AuthorDTO>(result);
                
                return CreatedAtAction(nameof(GetById), new { id = author.Id }, result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an author.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdateAuthorDTO updateAuthorDTO)
        {
            try
            {
                var existingAuthor = await _authorService.FindById(id);
                if (existingAuthor == null)
                {
                    return NotFound($"The author with ID {id} was not found.");
                }

                _mapper.Map(updateAuthorDTO, existingAuthor);

                var result = await _authorService.UpdateAuthor(id, existingAuthor);


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

                var authorDTO = _mapper.Map<AuthorDTO>(result.Value);
                return Ok(authorDTO);
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

                var authorDTO = _mapper.Map<GetAuthorDTO>(author);
                return Ok(authorDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the author with ID {id}.");
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