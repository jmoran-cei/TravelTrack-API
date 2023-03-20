using AutoMapper;
using TravelTrack_API.Domain;
using TravelTrack_API.Versions.v1.DtoModels;
using TripDto = TravelTrack_API.Versions.v2.DtoModels.TripDto;
using UserDto = TravelTrack_API.Versions.v2.DtoModels.UserDto;

namespace TravelTrack_API.Versions.v2.Profiles
{
    public class TripProfile : Profile
    {
        public TripProfile()
        {
            CreateMap<Destination, DestinationDto>();
            CreateMap<B2CUser, UserDto>()
                // User Id
                .ForMember(dto => dto.Id, opt => opt.MapFrom(domain => domain.Id))
                // Username
                .ForMember(dto => dto.Username, opt => opt.MapFrom(domain => domain.Username)) // username will always remain the same
                // First Name
                .ForMember(dto => dto.FirstName, opt => opt.MapFrom(domain => "")) // gets set later from microsoft graph result
                // Last Name
                .ForMember(dto => dto.LastName, opt => opt.MapFrom(domain => "")) // gets set later from microsoft graph result
                // Display Name
                .ForMember(dto => dto.DisplayName, opt => opt.MapFrom(domain => "")); // gets later set from microsoft graph result
            CreateMap<ToDo, ToDoDto>();
            CreateMap<Photo, PhotoDto>().ReverseMap();

            CreateMap<Trip, TripDto>()
                // Destinations
                .ForMember(dto => dto.Destinations,
                    opt => opt.MapFrom(t => t.Destinations.Select(td => td.Destination).ToList()))
                // Ignore mapping entity Trip.Members (old members version: pre-v3.0)
                .ForSourceMember(entity => entity.Members, opt => opt.DoNotValidate())
                // Members
                .ForMember(dto => dto.Members,
                        opt => opt.MapFrom(t => t.B2CMembers.Select(td => td.B2CUser).ToList()))
                // ToDo
                .ForMember(dto => dto.ToDo,
                        opt => opt.MapFrom(t => t.ToDo.ToList()))
                // Photos
                .ForMember(dto => dto.Photos,
                        opt => opt.MapFrom(t => t.Photos.ToList()));

            CreateMap<TripDto, TripDto>()
                // Destinations
                .ForMember(dto => dto.Destinations,
                    opt => opt.MapFrom(b2c => b2c.Destinations))
                // Members
                .ForMember(dto => dto.Members,
                        opt => opt.MapFrom(b2c => b2c.Members))
                // ToDo
                .ForMember(dto => dto.ToDo,
                        opt => opt.MapFrom(b2c => b2c.ToDo))
                // Photos
                .ForMember(dto => dto.Photos,
                        opt => opt.MapFrom(b2c => b2c.Photos));
        }
    }
}
