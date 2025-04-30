using Test_API.Domains;
using Test_API.DTO;
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

        public async Task<IEnumerable<GetBookDTO>> ListAsync(BookFilterDTO filter)
        {
            var query = _context.Books
                .Include(b => b.Author)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(b => b.Title.Contains(filter.Title));
            }

            if (filter.Price > 0)
            {
                query = query.Where(b => b.Price == filter.Price);
            }

            if (!string.IsNullOrEmpty(filter.Author))
            {
                query = query.Where(b => b.Author != null && b.Author.Name.Contains(filter.Author));
            }

            var books = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetBookDTO>>(books);
        }

        public async Task<ActionResult<BookDTO>> Post(BookDTO bookDTO)
        {
            var authorExists = await _context.Authors.AnyAsync(a => a.Id == bookDTO.AuthorId);
            if (!authorExists)
            {
                return new BadRequestObjectResult($"Author with ID {bookDTO.AuthorId} does not exist.");
            }

            var book = _mapper.Map<Book>(bookDTO);
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var updatedBookDTO = _mapper.Map<BookDTO>(book);
            return new ActionResult<BookDTO>(updatedBookDTO);
        }

        public async Task<ActionResult<BookDTO>> UpdateBook(int id, BookDTO bookDTO)
        {
            var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (existingBook == null)
            {
                return new NotFoundResult();
            }

            _mapper.Map(bookDTO, existingBook);
            _context.Entry(existingBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return new ConflictResult();
            }

            var UpdatedbookDTO = _mapper.Map<BookDTO>(existingBook);
            return new ActionResult<BookDTO>(UpdatedbookDTO);
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

        public async Task<GetBookDTO?> FindById(int id)
        {
            var query = from book in _context.Books
                        join author in _context.Authors
                        on book.AuthorId equals author.Id
                        where book.Id == id
                        select new
                        {
                            BookId = book.Id,
                            book.Title,
                            book.Price,
                            AuthorName = author.Name,
                            AuthorBio = author.Bio
                        };

            var result = await query.FirstOrDefaultAsync();

            if (result == null)
            {
                return null;
            }

            return new GetBookDTO
            {
                Id = result.BookId,
                Title = result.Title,
                Price = result.Price,
                AuthorName = result.AuthorName,
                AuthorBio = result.AuthorBio
            };
        }
    }
}