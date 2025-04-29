using Test_API.Models;
using Test_API.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Test_API.Interfaces;
using AutoMapper;

namespace Test_API.Services
{
    public class BookService : IBookService
    {
        private readonly BookdbContext _context;
        private readonly IMapper _mapper;

        public BookService(BookdbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> CountAsync(string? title = null, int? price = null)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.Contains(title));
            }

            if (price.HasValue)
            {
                query = query.Where(b => b.Price == price.Value);
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<GetBookDTO>> ListAsync(int page, int pageSize, string? title = null, int? price = null)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.Contains(title));
            }

            if (price.HasValue)
            {
                query = query.Where(b => b.Price == price.Value);
            }

            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetBookDTO>>(books);
        }

        public async Task<ActionResult<BookDTO>> Post(CreateBookDTO createBookDTO)
        {
            var book = _mapper.Map<Book>(createBookDTO);
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            var bookDTO = _mapper.Map<BookDTO>(book);
            return new ActionResult<BookDTO>(bookDTO);
        }

        public async Task<ActionResult<BookDTO>> UpdateBook(int id, UpdateBookDTO updateBookDTO)
        {
            var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (existingBook == null)
            {
                return new NotFoundResult();
            }

            _mapper.Map(updateBookDTO, existingBook);
            _context.Entry(existingBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return new ConflictResult();
            }

            var bookDTO = _mapper.Map<BookDTO>(existingBook);
            return new ActionResult<BookDTO>(bookDTO);
        }

        public async Task<IActionResult> DeleteBook(int id)
        {
            var bookToDelete = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (bookToDelete == null)
            {
                return new NotFoundResult();
            }

            _context.Books.Remove(bookToDelete);
            await _context.SaveChangesAsync();

            return new OkResult();
        }

        public async Task<BookDTO?> FindById(int id)
        {
            var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            return _mapper.Map<BookDTO?>(book);
        }
    }
}