using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelTrack_API.DTO;

public class Trip
{
    [Key]
    [Required]  
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required]
    public List<Destination> Destinations { get; set; } = new List<Destination>();
    [Required]
    public List<TripUser> Members { get; set; } = new List<TripUser>();
    public List<ToDo> ToDo { get; set; } = new List<ToDo>();
    [Required]
    public string ImgURL { get; set; } = string.Empty;
}

public class ToDo
{
    public int Id { get; set; }
    public string Task { get; set; } = string.Empty;
    public bool Complete { get; set; }
}

public class Destination
{
    [Required]
    public string Id { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}