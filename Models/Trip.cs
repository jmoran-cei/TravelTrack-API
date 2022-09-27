using System.ComponentModel.DataAnnotations;

namespace TravelTrack_API.Models;

public class Trip
{
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
    public List<User> Members { get; set; } = new List<User>();
    public List<ToDo> ToDo { get; set; } = new List<ToDo>();
    [Required]
    public string ImgURL { get; set; } = string.Empty;
}