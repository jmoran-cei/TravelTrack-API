using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelTrack_API.DTO;

public class TripDto
{ 
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<DestinationDto> Destinations { get; set; } = new List<DestinationDto>();
    public List<TripUserDto> Members { get; set; } = new List<TripUserDto>();
    public List<ToDoDto> ToDo { get; set; } = new List<ToDoDto>();
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
    public string Id { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}