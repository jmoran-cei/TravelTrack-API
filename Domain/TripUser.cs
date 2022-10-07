namespace TravelTrack_API.Domain
{
    public class TripUser
    {
        public long TripId { get; set; }
        public Trip Trip { get; set; } = new Trip();
        public string Username { get; set; } = string.Empty;
        public User User { get; set; } = new User();
    }
}
