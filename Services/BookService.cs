using Test_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Test_API.Services
{
    public class BookService : BaseService<Book>
    {
        public BookService(BookdbContext context) : base(context)
        {
        }
        public async Task<ActionResult<Book>> UpdateBookAsync(int id, Book book)
        {
            if (id != book.Id)
            {
                return new BadRequestResult();
            }

            var existingBook = await FindByIdAsync(id);
            if (existingBook == null)
            {
                return new NotFoundResult();
            }
            return await UpdateAsync(id, book);
        }

        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            var bookToDelete = await FindByIdAsync(id);
            if (bookToDelete == null)
            {
                return new NotFoundResult();
            }
            return await DeleteAsync(id);
        }
    }
}