using AutoMapper;
using TravelTrack_API.Domain;
using TravelTrack_API.Versions.v1.DtoModels;

namespace TravelTrack_API.Versions.v1.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
