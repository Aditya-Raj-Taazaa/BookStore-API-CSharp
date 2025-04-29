using AutoMapper;
using Test_API.Models;

namespace Test_API.Models.DTOs
{
    
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public string AuthorName { get; set; }
    }

    
    public class CreateBookDTO
    {
        public string Title { get; set; }
        public int Price { get; set; }
        public int AuthorId { get; set; }
    }

    public class UpdateBookDTO
    {
        public string Title { get; set; }
        public int Price { get; set; }
    }
    public class GetBookDTO
    {
        public string Title { get; set; }
        public int Price { get; set; }
    }
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookDTO>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? src.Author.Name : string.Empty));
            CreateMap<CreateBookDTO, Book>();
            CreateMap<UpdateBookDTO, Book>();
            CreateMap<Book, GetBookDTO>();
        }
    }
}