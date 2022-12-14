using AutoMapper;
using TravelTrack_API.Domain;
using TravelTrack_API.DTO;

namespace TravelTrack_API.Profiles
{
    public class TripProfile : Profile
    {
        public TripProfile()
        {
            CreateMap<Destination, DestinationDto>();
            CreateMap<User, TripUserDto>();
            CreateMap<ToDo, ToDoDto>();

            CreateMap<Trip, TripDto>()
                // Destinations
                .ForMember(dto => dto.Destinations,
                    opt => opt.MapFrom(t => t.Destinations.Select(td => td.Destination).ToList()))
                // Members
                .ForMember(dto => dto.Members,
                        opt => opt.MapFrom(t => t.Members.Select(td => td.User).ToList()))
                // ToDo
                .ForMember(dto => dto.ToDo,
                        opt => opt.MapFrom(t => t.ToDo.ToList()));
        }
    }
}
