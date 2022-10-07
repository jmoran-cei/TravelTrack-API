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
    public List<TripDestination> Destinations { get; set; } = new List<TripDestination>();
    [Required]
    public List<TripUser> Members { get; set; } = new List<TripUser>();
    public List<ToDo> ToDo { get; set; } = new List<ToDo>();
}

public class ToDo
{
    public int Id { get; set; }
    public string Task { get; set; } = string.Empty;
    public bool Complete { get; set; }
    public long TripId { get; set; }
    public Trip Trip { get; set; } = new Trip();
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