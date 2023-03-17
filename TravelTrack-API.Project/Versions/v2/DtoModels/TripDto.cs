using TravelTrack_API.Versions.v1.DtoModels;

namespace TravelTrack_API.Versions.v2.DtoModels
{
    /* * * * * * * * * * *
     * Preface: v2 change in Trip domain entity. It still has Trip.Members as before (to maintain functionality with v1); 
     * However, Trip.B2CMembers was added as a new sub-entity of Trip in order to get new model mapping with AD B2C user objects.
     * 
     * V2 TripDto differentiation:
     * Trip.Members have different mapping as UserDto changed from v1. 
     * Appropriate changed mappings can be seen in v2/Profiles
    * * * * * * * * * * */
    public class TripDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DestinationDto> Destinations { get; set; }
        // uses v2.Models.UserDto
        public List<UserDto> Members { get; set; }
        public List<ToDoDto> ToDo { get; set; } = new List<ToDoDto>();
        public List<PhotoDto> Photos { get; set; } = new List<PhotoDto>();
        public string ImgURL { get; set; } = string.Empty;
        public TripDto()
        {
            Destinations = new List<DestinationDto>();
            // uses v2.Models.UserDto
            Members = new List<UserDto>();
        }
    }
}