using Test_API.Domains;
using Test_API.Interfaces;

namespace Test_API.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(BookdbContext context) : base(context)
        {
        }
    }
}