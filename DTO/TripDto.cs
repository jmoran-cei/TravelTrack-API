using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelTrack_API.DTO;

public class TripDto
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
    public List<DestinationDto> Destinations { get; set; } = new List<DestinationDto>();
    [Required]
    public List<TripUserDto> Members { get; set; } = new List<TripUserDto>();
    public List<ToDoDto> ToDo { get; set; } = new List<ToDoDto>();
    [Required]
    public string ImgURL { get; set; } = string.Empty;
}

public class ToDoDto
{
    public int Id { get; set; }
    public string Task { get; set; } = string.Empty;
    public bool Complete { get; set; }
}

public class DestinationDto
{
    [Required]
    public string Id { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}