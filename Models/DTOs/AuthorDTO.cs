using AutoMapper;
using Test_API.Models;

namespace Test_API.Models.DTOs
{
    public class AuthorDTO
    {
        public int Id { get; set; }
        required public string  Name { get; set; }
        required public string Bio { get; set; }
    }
     
    public class CreateAuthorDTO
    {
        required public string Name { get; set; }
        required public string Bio { get; set; }
    }

    public class UpdateAuthorDTO
    {
        required public string Name { get; set; }
        required public string Bio { get; set; }
    }
     
    public class GetAuthorDTO
    {
        required public string Name { get; set; }
    }
     
    public class AuthorMappingProfile : Profile
    {
        public AuthorMappingProfile()
        {
            CreateMap<Author, AuthorDTO>();   
            CreateMap<Author, GetAuthorDTO>();
            CreateMap<CreateAuthorDTO, Author>();
            CreateMap<UpdateAuthorDTO, Author>();
        }
    }
}