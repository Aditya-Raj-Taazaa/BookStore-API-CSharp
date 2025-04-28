using AutoMapper;
using Test_API.Models;

namespace Test_API.Models.DTOs
{
    // DTO for returning book data to the client
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public string AuthorName { get; set; }
    }

    // DTO for creating a new book
    public class CreateBookDTO
    {
        public string Title { get; set; }
        public int Price { get; set; }
        public int AuthorId { get; set; }
    }

    // DTO for updating an existing book
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

    // AutoMapper profile for mapping between entities and DTOs
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookDTO>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name));
            CreateMap<CreateBookDTO, Book>();
            CreateMap<UpdateBookDTO, Book>();
            CreateMap<Book, GetBookDTO>();
        }
    }
}