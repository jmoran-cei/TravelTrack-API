using AutoMapper;
using TravelTrack_API.Domain;
using TravelTrack_API.Versions.v2.Models;

namespace TravelTrack_API.Versions.v2.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
