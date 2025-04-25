using Test_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Test_API.Services
{
    public class AuthorService : BaseService<Author>
    {
        private readonly FormatterService _formatterService;

        public AuthorService(BookdbContext context, FormatterService formatterService)
            : base(context)
        {
            _formatterService = formatterService;
        }

        public async Task<ActionResult<Author>> CreateAuthorAsync(Author author)
        {
            Console.WriteLine($"Original Bio: {author.Bio}");
            author.Bio = _formatterService.BioFormat(author.Bio);
            Console.WriteLine($"Formatted Bio: {author.Bio}");
            return await CreateAsync(author);
        }
    }
}