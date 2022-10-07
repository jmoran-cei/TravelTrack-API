namespace TravelTrack_API.Domain
{
    public class TripDestination
    {
        public long TripId { get; set; }
        public Trip Trip { get; set; } = null!;
        public string DestinationId { get; set; } = string.Empty;
        public Destination Destination { get; set; } = null!;
    }
}
