using System.Linq;
using AutoMapper;
using ProfileService.Api.Application.Dtos;
using ProfileService.Api.Application.Extension;
using ProfileService.Api.Domain.Entities;

namespace ProfileService.Api.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // source -> destination
            CreateMap<ProfileUser, ProfileDto>()
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
            .ForMember(dest => dest.photoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.isMain).Url));
            CreateMap<ProfileDto, ProfileUser>();
            CreateMap<Photo, PhotoDto>().ReverseMap();
            CreateMap<ProfileUser, CreateProfileDto>().ReverseMap();
            CreateMap<ProfileUser, UpdateProfileDto>().ReverseMap();

            
        }
    }
}
