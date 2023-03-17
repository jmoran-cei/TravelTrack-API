using AutoMapper;
using TravelTrack_API.Domain;
using TravelTrack_API.Versions.v2.DtoModels;

namespace TravelTrack_API.Versions.v2.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<B2CUser, UserDto>().ReverseMap();
        }
    }
}
