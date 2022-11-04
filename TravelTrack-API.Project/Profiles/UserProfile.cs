using AutoMapper;
using TravelTrack_API.Domain;
using TravelTrack_API.DTO;

namespace TravelTrack_API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
