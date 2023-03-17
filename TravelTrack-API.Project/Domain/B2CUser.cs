using System.ComponentModel.DataAnnotations;

namespace TravelTrack_API.Domain;
public class B2CUser // v2
{
    [Key]
    [Required]
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public List<TripB2CUser> Trips { get; set; } = new List<TripB2CUser>();
}