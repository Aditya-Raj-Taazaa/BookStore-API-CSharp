using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Test_API.Interfaces;

namespace Test_API.Services
{
    public class GenericService<TEntity, TGetDTO, TCreateDTO, TUpdateDTO> : IGenericService<TEntity, TGetDTO, TCreateDTO, TUpdateDTO>
        where TEntity : class
    {
        private readonly DbContext _context;
        private readonly IMapper _mapper;
        private readonly DbSet<TEntity> _dbSet;

        public GenericService(DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<IEnumerable<TGetDTO>> ListAsync(int page, int pageSize)
        {
            var entities = await _dbSet
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TGetDTO>>(entities);
        }

        public async Task<ActionResult<TGetDTO>> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return new NotFoundResult();
            }

            return new ActionResult<TGetDTO>(_mapper.Map<TGetDTO>(entity));
        }

        public async Task<ActionResult<TGetDTO>> CreateAsync(TCreateDTO createDTO)
        {
            var entity = _mapper.Map<TEntity>(createDTO);
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();

            return new ActionResult<TGetDTO>(_mapper.Map<TGetDTO>(entity));
        }

        public async Task<ActionResult<TGetDTO>> UpdateAsync(int id, TUpdateDTO updateDTO)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return new NotFoundResult();
            }

            _mapper.Map(updateDTO, entity);
            _context.Entry(entity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return new ConflictResult();
            }

            return new ActionResult<TGetDTO>(_mapper.Map<TGetDTO>(entity));
        }

        public async Task<IActionResult> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return new NotFoundResult();
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();

            return new OkResult();
        }
    }
}