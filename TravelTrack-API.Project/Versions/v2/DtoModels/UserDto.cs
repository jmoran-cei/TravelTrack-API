namespace TravelTrack_API.Versions.v2.DtoModels
{
    public class UserDto
    {
        // represents a member of a trip
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}