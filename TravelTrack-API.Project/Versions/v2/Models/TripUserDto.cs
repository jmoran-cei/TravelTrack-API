namespace TravelTrack_API.Versions.v2.Models
{
    // represents a member of a trip
    public class TripUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
