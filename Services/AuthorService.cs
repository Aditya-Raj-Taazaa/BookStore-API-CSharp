using Test_API.Domains;
using Test_API.DTO;
using Microsoft.AspNetCore.Mvc;
using Test_API.Interfaces;
using AutoMapper;

namespace Test_API.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public AuthorService(IAuthorRepository authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        public async Task<int> CountAsync(string? name = null, string? bio = null)
        {
            return await _authorRepository.CountAsync(a =>
                (string.IsNullOrEmpty(name) || a.Name.Contains(name)) &&
                (string.IsNullOrEmpty(bio) || a.Bio.Contains(bio)));
        }

        public async Task<IEnumerable<GetAuthorDTO>> ListAsync(AuthorFilterDTO filters)
        {
            var authors = await _authorRepository.FindAsync(a =>
                (string.IsNullOrEmpty(filters.Name) || a.Name.Contains(filters.Name)) &&
                (string.IsNullOrEmpty(filters.Bio) || a.Bio.Contains(filters.Bio)));

            var paginatedAuthors = authors
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize);

            return _mapper.Map<IEnumerable<GetAuthorDTO>>(paginatedAuthors);
        }

        public async Task<ActionResult<AuthorDTO>> Post(AuthorDTO authorDTO)
        {
            var author = _mapper.Map<Author>(authorDTO);
            await _authorRepository.AddAsync(author);

            var createdAuthorDTO = _mapper.Map<AuthorDTO>(author);
            return new ActionResult<AuthorDTO>(createdAuthorDTO);
        }

        public async Task<ActionResult<AuthorDTO>> UpdateAuthor(int id, AuthorDTO authorDTO)
        {
            var existingAuthor = await _authorRepository.GetByIdAsync(id);
            if (existingAuthor == null)
            {
                return new NotFoundResult();
            }

            _mapper.Map(authorDTO, existingAuthor);
            _authorRepository.Update(existingAuthor);

            var updatedAuthorDTO = _mapper.Map<AuthorDTO>(existingAuthor);
            return new ActionResult<AuthorDTO>(updatedAuthorDTO);
        }

        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var authorToDelete = await _authorRepository.GetByIdAsync(id);
            if (authorToDelete == null)
            {
                return new NotFoundResult();
            }

            _authorRepository.Remove(authorToDelete);
            return new OkResult();
        }

        public async Task<GetAuthorDTO?> FindById(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            return _mapper.Map<GetAuthorDTO?>(author);
        }
    }
}