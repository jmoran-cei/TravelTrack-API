namespace TravelTrack_API.Domain
{
    public class TripB2CUser
    {
        public long TripId { get; set; }
        public Trip Trip { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public B2CUser B2CUser { get; set; } = null!;
    }
}
