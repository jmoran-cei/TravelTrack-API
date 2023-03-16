using AutoMapper;
using TravelTrack_API.Domain;
using TravelTrack_API.Versions.v3.Models;

namespace TravelTrack_API.Versions.v3.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<B2CUser, UserDto>().ReverseMap();
        }
    }
}
