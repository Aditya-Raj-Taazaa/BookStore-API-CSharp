using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test_API.Models;
using Test_API.ActionFilters;
using Test_API.Services;

namespace Test_API.Controllers
{
    [ApiController]
    [Route("api/Locations")]
    public class LocationController : ControllerBase
    {
        private readonly BookdbContext _context; 
        private readonly ILogger<LocationController> _logger;
        public readonly LocationService _locationService;

        public LocationController(BookdbContext context, ILogger<LocationController> logger, LocationService locationService) // Change parameter type to LocationService
        {
            _context = context;
            _logger = logger;
            _locationService = locationService;
        }
       
        

        [ExecutionTimeFilter]
        [HttpGet(Name = "GetLocationDetails")]
        
        public async Task<IActionResult> Get()
        {
             try
            {
                var locations = await _locationService.ListAsync();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching location details.");
                return StatusCode(500, "Internal server error");
            }
        }

        
        [HttpPost]
        public async Task<ActionResult<Location>> Post(Location location)
        {
            try{
                return await _locationService.Post(location);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"Error on Creating Location");
                return StatusCode(500,"Internal Server Error");
            }
            
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Location location)
        {
            var result = await _locationService.UpdateLocation(id, location);

            if (result.Result is BadRequestResult)
            {
                return BadRequest("The provided ID does not match the location ID.");
            }
            if (result.Result is NotFoundResult)
            {
                return NotFound("The location with the specified ID was not found.");
            }
            if (result.Result is ConflictResult)
            {
                return Conflict("A concurrency issue occurred while updating the location.");
            }
            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var Location = await _locationService.DeleteLocation(id);

                if (Location is NotFoundResult)
                {
                    return NotFound($"The Location not Found By ID : {id}");
                }
                return Location;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting Location Details");
                return StatusCode(500, "Internal server error");
            }
        }

        
        
        [ExecutionTimeFilter]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocation(int id)
        {
            try
            {
                var author = await _locationService.FindById(id);
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

        [ExecutionTimeFilter]
        [HttpGet("count")]
        public async Task<IActionResult> CountLocations()
        {
            try
            {
                var count = await _context.Locations.CountAsync();
                Console.WriteLine(count);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting Locations.");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool LocationExists(int id)
        {
            var Location = _context.Locations.Find(id);
            return Location != null;
        }
    }
}
