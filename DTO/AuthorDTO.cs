using AutoMapper;
using Test_API.Domains;

namespace Test_API.DTO
{
    public class AuthorDTO
    {
        public int Id { get; set; }
         public string  Name { get; set; }
         public string Bio { get; set; }
    }
     
    public class GetAuthorDTO
    {
         public string Name { get; set; }
    }
     
    public class AuthorMappingProfile : Profile
    {
        public AuthorMappingProfile()
        {
            CreateMap<Author, AuthorDTO>();   
            CreateMap<Author, GetAuthorDTO>();
            CreateMap<AuthorDTO,Author>() // for create and update
                    .ForMember(dest => dest.Id, opt => opt.Ignore()); //ignores Id when overwriting e.g :(Update func.)
        }
    }
}