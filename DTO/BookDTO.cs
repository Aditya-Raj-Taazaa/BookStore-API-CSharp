using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Test_API.Domains;

namespace Test_API.DTO
{
    
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public int AuthorId {get; set;}
        public string ?AuthorName {get; set;}
    }

    public class GetBookDTO
    {
        public string Title { get; set; }
        public int Price { get; set; }

        public string ?AuthorName {get; set;}
    }
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, GetBookDTO>();

            CreateMap<Book, BookDTO>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? src.Author.Name : string.Empty));
            
            CreateMap<BookDTO,Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); 
        }
    }
}