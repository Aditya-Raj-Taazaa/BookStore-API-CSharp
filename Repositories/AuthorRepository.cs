using Test_API.Domains;
using Test_API.Interfaces;

namespace Test_API.Repositories
{
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        public AuthorRepository(BookdbContext context) : base(context)
        {
        }
    }
}