namespace TravelTrack_API.Versions.v3.Models
{
    public class TripDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DestinationDto> Destinations { get; set; }
        public List<UserDto> Members { get; set; }
        public List<ToDoDto> ToDo { get; set; } = new List<ToDoDto>();
        public List<PhotoDto> Photos { get; set; } = new List<PhotoDto>();
        public string ImgURL { get; set; } = string.Empty;
        public TripDto()
        {
            Destinations = new List<DestinationDto>();
            Members = new List<UserDto>();
        }
    }

    public class ToDoDto
    {
        public int Id { get; set; }
        public string Task { get; set; } = string.Empty;
        public bool Complete { get; set; }
    }

    public class DestinationDto
    {
        public string Id { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class PhotoDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string AddedByUser { get; set; } = string.Empty;
        public string Alt { get; set; } = string.Empty;
        public long TripId { get; set; }
    }
}