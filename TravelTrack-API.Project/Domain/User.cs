using System.ComponentModel.DataAnnotations;

namespace TravelTrack_API.Domain;

public class User
{
    [Key]
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    public List<TripUser> Trips { get; set; } = new List<TripUser>();
}