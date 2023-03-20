namespace TravelTrack_API.MicrosoftGraphModels
{
    public class MicrosoftGraphUser
    {
        public string Id { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string GivenName { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public List<MicrosoftGraphUserIdentity> Identities { get; set; } = null!;
    }
}
