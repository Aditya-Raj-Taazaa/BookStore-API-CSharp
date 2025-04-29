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
        private readonly FormatterService _formatterService;
        private readonly IMapper _mapper;

        public AuthorService(BookdbContext context, FormatterService formatterService, IMapper mapper)
        {
            _context = context;
            _formatterService = formatterService;
            _mapper = mapper;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Authors.CountAsync();
        }

        public async Task<IEnumerable<GetAuthorDTO>> ListAsync(int page, int pageSize)
        {
            var authors = await _context.Authors
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return _mapper.Map<IEnumerable<GetAuthorDTO>>(authors);
        }

        public async Task<ActionResult<AuthorDTO>> Post(CreateAuthorDTO createAuthorDTO)
        {
            var author = _mapper.Map<Author>(createAuthorDTO);
            author.Bio = _formatterService.BioFormat(author.Bio);
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            var authorDTO = _mapper.Map<AuthorDTO>(author);
            return new ActionResult<AuthorDTO>(authorDTO);
        }

        public async Task<ActionResult<AuthorDTO>> UpdateAuthor(int id, UpdateAuthorDTO updateAuthorDTO)
        {
            var existingAuthor = await FindById(id);
            if (existingAuthor == null)
            {
                return new NotFoundResult();
            }
            _mapper.Map(updateAuthorDTO, existingAuthor);
            _context.Entry(existingAuthor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("Concurrency exception: " + ex.Message);
                return new ConflictResult();
            }
            var authorDTO = _mapper.Map<AuthorDTO>(existingAuthor);
            return new ActionResult<AuthorDTO>(authorDTO);
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
            var authorDTO = _mapper.Map<AuthorDTO>(authorToDelete);
            return new OkObjectResult(authorDTO);
        }

        public async Task<GetAuthorDTO?> FindById(int id)
        {
            var author = await _context.Authors.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            return _mapper.Map<GetAuthorDTO?>(author);
        }
    }
}