using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Test_API.Models;
namespace Test_API.Services
{
    public class BaseService<T> where T : class
    {
        protected readonly BookdbContext _context;

        public BaseService(BookdbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> ListAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T?> FindByIdAsync(int id)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<ActionResult<T>> CreateAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return new ActionResult<T>(entity);
        }

        public async Task<ActionResult<T>> UpdateAsync(int id, T entity)
        {
            var trackedEntity = await _context.Set<T>().FindAsync(id);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached; // Detach the tracked entity
            }

            _context.Entry(entity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"Concurrency exception: {ex.Message}");
                return new ConflictResult();
            }

            return new ActionResult<T>(entity);
        }

        public async Task<IActionResult> DeleteAsync(int id)
        {
            var entity = await FindByIdAsync(id);
            if (entity == null)
            {
                return new NotFoundResult();
            }

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return new OkObjectResult(entity);
        }
    }
}