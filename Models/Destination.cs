using System.ComponentModel.DataAnnotations;

namespace TravelTrack_API.Models;

public class Destination
{
    [Key]
    [Required]
    public string DestinationId { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}