using AutoMapper;
using TravelTrack_API.Domain;
using TravelTrack_API.Versions.v1.DtoModels;

namespace TravelTrack_API.Versions.v1.Profiles
{
    public class TripProfile : Profile
    {
        public TripProfile()
        {
            CreateMap<Destination, DestinationDto>();
            CreateMap<User, TripUserDto>();
            CreateMap<ToDo, ToDoDto>();
            CreateMap<Photo, PhotoDto>().ReverseMap();

            CreateMap<Trip, TripDto>()
                // Destinations
                .ForMember(dto => dto.Destinations,
                    opt => opt.MapFrom(t => t.Destinations.Select(td => td.Destination).ToList()))
                // Members
                .ForMember(dto => dto.Members,
                        opt => opt.MapFrom(t => t.Members.Select(td => td.User).ToList()))
                // ToDo
                .ForMember(dto => dto.ToDo,
                        opt => opt.MapFrom(t => t.ToDo.ToList()))
                // Ignore mapping entity Trip.B2CMembers (newer members version: v3.0+)
                .ForSourceMember(entity => entity.B2CMembers, opt => opt.DoNotValidate())
                // Photos
                .ForMember(dto => dto.Photos,
                        opt => opt.MapFrom(t => t.Photos.ToList()));
        }
    }
}
