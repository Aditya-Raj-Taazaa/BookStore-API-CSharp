using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test_API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Test_API.Controllers
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LoggerResourceFilter : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            Console.WriteLine(" ➡️ Request is Starting ➡️");
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            Console.WriteLine(" ✅ Request is Completed ✅");
        }
        
    }


    [ApiController]
    [Route("api/Authors")]
    
    public class AuthorController : ControllerBase
    {
        private readonly BookdbContext _context;
        private readonly ILogger<AuthorController> _logger;
        private readonly IConfiguration _configuration;

        public AuthorController(BookdbContext context, ILogger<AuthorController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet(Name = "GetUserDetails")]
        [LoggerResourceFilter]
        public async Task<IEnumerable<Author>> Get()
        {
            return await _context.Authors.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Author>> Post(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = author.Id }, author);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Author author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }

            _context.Entry(author).State = EntityState.Modified;
            try
            {
                if (!AuthorExists(id)) 
                {
                    return NotFound();
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok();
        }
        private bool AuthorExists(int id)
        {
            //return _context.Books.Any(e => e.Id == id);
            var Author = _context.Authors.Find(id);
            return Author != null;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    return NotFound();
                }

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                return StatusCode(204);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting User Details");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get_By_Id(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    return NotFound();
                }
                return Ok(author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting User details with {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("count")]
        
        public async Task<IActionResult> Count_Authors()
        {
            try
            {
                var count = await _context.Authors.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting Authors.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}