using System.ComponentModel.DataAnnotations;

namespace TravelTrack_API.Models;

public class ToDo
{
    [Required]
    public string Task { get; set; } = string.Empty;
    [Required]
    public bool Complete { get; set; }
}