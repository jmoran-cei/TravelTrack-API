using System.ComponentModel.DataAnnotations;

namespace TravelTrack_API.Domain;

public class Trip
{
    [Key]
    [Required]
    public long Id { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required]
    public string ImgURL { get; set; } = string.Empty;
    [Required]
    public List<TripDestination> Destinations { get; set; }
    public List<TripUser> Members { get; set; }
    public List<TripB2CUser> B2CMembers { get; set; } // v2
    public List<ToDo> ToDo { get; set; } = new List<ToDo>();
    public List<Photo> Photos { get; set; } = new List<Photo>();

    public Trip()
    {
        Destinations = new List<TripDestination>();
        Members = new List<TripUser>();
        B2CMembers = new List<TripB2CUser>(); // v2
    }
}

public class ToDo
{
    public int Id { get; set; }
    public string Task { get; set; } = string.Empty;
    public bool Complete { get; set; }
    public long TripId { get; set; }
    public Trip Trip { get; set; } = null!;
}

public class Destination
{
    [Required]
    public string Id { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public List<TripDestination> Trips { get; set; } = new List<TripDestination>();
}

public class Photo
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string AddedByUser { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;
    public long TripId { get; set; }
    public Trip Trip { get; set; } = null!;
}