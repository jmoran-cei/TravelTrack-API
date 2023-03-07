using TravelTrack_API.MicrosoftGraphModels;

namespace TravelTrack_API.Services.MicrosoftGraph
{
    public interface IMicrosoftGraphService
    {
        HttpClient SetupGraphClient(); 
        Task<HttpResponseMessage> RequestUserByUsernameAsync(string username);
        Task<HttpResponseMessage> RequestUserByIdAsync(string id);
        Task<HttpResponseMessage> RequestUserIdentitiesAsync();
    }
}
