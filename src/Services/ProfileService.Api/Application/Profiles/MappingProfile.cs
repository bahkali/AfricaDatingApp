using AutoMapper;
using ProfileService.Api.Application.Dtos;
using ProfileService.Api.Domain.Entities;

namespace ProfileService.Api.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // source -> destination
            CreateMap<ProfileUser, ProfileDto>().ReverseMap();
            CreateMap<ProfileUser, CreateProfileDto>().ReverseMap();
            CreateMap<ProfileUser, UpdateProfileDto>().ReverseMap();

            
        }
    }
}
