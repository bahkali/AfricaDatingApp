using AutoMapper;

namespace AuthService.Api.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // source -> destination
            //CreateMap<LeaveRequest, LeaveRequestDto>().ReverseMap();
        }
    }
}
