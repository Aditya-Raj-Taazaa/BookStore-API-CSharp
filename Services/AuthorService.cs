using Test_API.Models;
using Test_API.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Test_API.Interfaces;
using AutoMapper;

namespace Test_API.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly BookdbContext _context;
        private readonly IMapper _mapper;

        public AuthorService(BookdbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> CountAsync(string? name = null, string? bio = null)
        {
            var query = _context.Authors.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(a => a.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(bio))
            {
                query = query.Where(a => a.Bio.Contains(bio));
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<GetAuthorDTO>> ListAsync(int page, int pageSize, string? name = null, string? bio = null)
        {
            var query = _context.Authors.AsQueryable();
            if (!string.IsNullOrEmpty(name))
                query = query.Where(a => a.Name.Contains(name));

            if (!string.IsNullOrEmpty(bio))
                query = query.Where(a => a.Bio.Contains(bio));

            var authors = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return _mapper.Map<IEnumerable<GetAuthorDTO>>(authors);
        }

        public async Task<ActionResult<AuthorDTO>> Post(AuthorDTO authorDTO)
        {
            var author = _mapper.Map<Author>(authorDTO);
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var createdAuthorDTO = _mapper.Map<AuthorDTO>(author);
            return new ActionResult<AuthorDTO>(createdAuthorDTO);
        }

        public async Task<ActionResult<AuthorDTO>> UpdateAuthor(int id, AuthorDTO authorDTO)
        {
            var existingAuthor = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (existingAuthor == null)
            {
                return new NotFoundResult();
            }

            _mapper.Map(authorDTO, existingAuthor);
            _context.Entry(existingAuthor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return new ConflictResult();
            }
            var updatedAuthorDTO = _mapper.Map<AuthorDTO>(existingAuthor);
            return new ActionResult<AuthorDTO>(updatedAuthorDTO);
        }

        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var authorToDelete = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (authorToDelete == null)
            {
                return new NotFoundResult();
            }

            _context.Authors.Remove(authorToDelete);
            await _context.SaveChangesAsync();

            return new OkResult();
        }

        public async Task<GetAuthorDTO?> FindById(int id)
        {
            var author = await _context.Authors.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            return _mapper.Map<GetAuthorDTO?>(author);
        }
    }
}