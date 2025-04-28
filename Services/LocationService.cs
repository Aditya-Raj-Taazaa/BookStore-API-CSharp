
using Test_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Test_API.Services
{
    public class LocationService(BookdbContext context)
    {
        private readonly BookdbContext _context = context;

        public async Task<IEnumerable<Location>> ListAsync()
        {
            return await _context.Locations.ToListAsync();
        }

        public async Task<ActionResult<Location>> Post(Location Location)
        {
            _context.Locations.Add(Location);
            await _context.SaveChangesAsync();
            return new ActionResult<Location>(Location);
        }

        public async Task<ActionResult<Location>> UpdateLocation(int id, Location Location)
        {
            if (id != Location.Id)
            {
                return new BadRequestResult();
            }

            var existingLocation = await FindById(id);
            if (existingLocation == null)
            {
                return new NotFoundResult();
            }

            _context.Entry(Location).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(); // Ensure this is awaited
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("Concurrency exception: " + ex.Message);
                return new ConflictResult();
            }

            return new ActionResult<Location>(Location);
            
        }

        public async Task<IActionResult> DeleteLocation(int id)
        {
            var LocationToDelete = await FindById(id);
            if(LocationToDelete == null)
            {
                return new NotFoundResult();
            }

            _context.Locations.Remove(LocationToDelete);
            await _context.SaveChangesAsync();
            return new OkObjectResult(LocationToDelete);  
        }

        public async Task<bool> LocationExists(int id)
        {
            return await _context.Locations.AnyAsync(b => b.Id == id);
        }

        public async Task<Location?> FindById(int id)
        {
            return await _context.Locations.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        }

        public static implicit operator LocationService(BookService v)
        {
            throw new NotImplementedException();
        }
    }
}
