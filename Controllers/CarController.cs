using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test_API.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Test_API.Controllers
{
    [ApiController]
    [Route("api/cars")]
    public class CarController : ControllerBase
    {
        private readonly CarContext _context;
        private readonly ILogger<CarController> _logger;
        public CarController(CarContext context, ILogger<CarController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet(Name = "GetCarDetails")]
        public async Task<IEnumerable<Car>> Get()
        {
            return await _context.Cars.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Car>> Post(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = car.Id }, car);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id,Car car)
        {
            try
            {
                if (id != car.Id)
                {
                    return BadRequest();
                }

                _context.Entry(car).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(car);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var car = await _context.Cars.FindAsync(id);
                if (car == null)
                {
                    return NotFound();
                }

                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();

                return StatusCode(204);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting Car Details");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get_By_Id(int id)
        {
            try
            {
                var car = await _context.Cars.FindAsync(id);
                if (car == null)
                {
                    return NotFound();
                }
                return Ok(car);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting car details with {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count_Cars()
        {
            try
            {
                var count = await _context.Cars.CountAsync();
                Console.WriteLine(count);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting cars.");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool CarExists(int id)
        {
            //return _context.Cars.Any(e => e.Id == id);
            var car = _context.Cars.Find(id);
            return car != null;
        }
    }
}
