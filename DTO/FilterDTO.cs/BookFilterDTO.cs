using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Test_API.Domains;

namespace Test_API.DTO
{
    
    public class BookFilterDTO
    {
        public int Page { get; set; }
        public string? Title { get; set; }
        public int PageSize { get; set; }
        public int? Price {get; set;}
        public string? Author {get; set;}
    } 
}