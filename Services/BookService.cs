using Test_API.Domains;
using Test_API.DTO;
using Microsoft.AspNetCore.Mvc;
using Test_API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Test_API.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

            public async Task<int> CountAsync(string? title = null, int? price = null)
            {
            return await _bookRepository.CountAsync(b =>
                    (string.IsNullOrEmpty(title) || b.Title.Contains(title)) &&
                    (!price.HasValue || b.Price == price));
            }

        public async Task<IEnumerable<GetBookDTO>> ListAsync(BookFilterDTO filter)
{
    var books = await _bookRepository.FindAsync(b =>
        (string.IsNullOrEmpty(filter.Title) || b.Title.Contains(filter.Title)) &&
        (filter.Price <= 0 || b.Price == filter.Price),
        include: query => query.Include(b => b.Author)); 

    return _mapper.Map<IEnumerable<GetBookDTO>>(books);
}

        public async Task<ActionResult<BookDTO>> Post(BookDTO bookDTO)
        {
            var authorExists = await _authorRepository.GetByIdAsync(bookDTO.AuthorId) != null;
            if (!authorExists)
            {
                return new BadRequestObjectResult($"Author with ID {bookDTO.AuthorId} does not exist.");
             }

            var book =_mapper.Map<Book>(bookDTO);
            await _bookRepository.AddAsync(book);
            return new ActionResult<BookDTO>(_mapper.Map<BookDTO>(book));
        }

        public async Task<ActionResult<BookDTO>> UpdateBook(int id, BookDTO bookDTO)
        {
            var existingBook = await _bookRepository.GetByIdAsync(id);
            if (existingBook == null)
            {
                    return new NotFoundResult();
            }

            _mapper.Map(bookDTO, existingBook);
            _bookRepository.Update(existingBook);

            return new ActionResult<BookDTO>(_mapper.Map<BookDTO>(existingBook));
        }

        public async Task<IActionResult> DeleteBook(int id)
        {
            var bookToDelete = await _bookRepository.GetByIdAsync(id);
            if (bookToDelete == null)
            {
                return new NotFoundResult();
            }

            _bookRepository.Remove(bookToDelete);
            return new OkResult();
        }

        public async Task<GetBookDTO?> FindById(int id)
        {
            var query = from book in _bookRepository.GetAllAsync().Result
                        join author in _authorRepository.GetAllAsync().Result
                        on book.AuthorId equals author.Id
                        where book.Id == id
                        select new GetBookDTO
                        {
                            Id = book.Id,
                            Title = book.Title,
                            Price = book.Price,
                            AuthorName = author.Name,
                            AuthorBio = author.Bio
                        };

            return query.FirstOrDefault();
        }
    }
}