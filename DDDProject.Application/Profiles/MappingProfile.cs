using AutoMapper;
using DDDProject.Domain.Dtos.BookDto;
using DDDProject.Domain.Dtos.GenreDto;
using DDDProject.Domain.Entities;

namespace DDDProject.Domain.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BookDto, BookForm>().ReverseMap();
            CreateMap<BookDto, Book>()
            .ForMember(dest => dest.Genre, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre.Name));
            CreateMap<BookForm, Book>().ReverseMap();

            CreateMap<GenreDto, GenreForm>().ReverseMap();
            CreateMap<GenreDto, Genre>().ReverseMap();
            CreateMap<GenreForm, Genre>().ReverseMap();
        }
    }
}
