using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Test_API.Domains;

namespace Test_API.DTO
{
    
    public class AuthorFilterDTO
    {
        public int Page { get; set; }
        public string? Name { get; set; }
        public int PageSize { get; set; }
        public string? Bio {get; set;}
    }
}