using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Text;
using TravelTrack_API.MicrosoftGraphModels;

namespace TravelTrack_API.SharedServices.MicrosoftGraph
{
    public class MicrosoftGraphService : IMicrosoftGraphService
    {
        private readonly string tenantId;
        private readonly string clientId;
        private readonly string clientSecret;
        public MicrosoftGraphService(IConfiguration configuration)
        {
            tenantId = configuration["AzureAdB2C:TenantId"];
            clientId = configuration["AzureAdB2C:ClientId"];
            clientSecret = configuration["AzureAdB2C:ClientSecret"];
        }


        public HttpClient SetupGraphClient()
        {
            HttpClient client = new HttpClient();

            // get access token from Azure AD 
            var reqContent = @"grant_type=client_credentials&resource=https://graph.microsoft.com&client_id=" + clientId + "&client_secret=" + System.Web.HttpUtility.UrlEncode(clientSecret);
            var Content = new StringContent(reqContent, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = client.PostAsync("https://login.microsoftonline.com/" + tenantId + "/oauth2/token", Content).Result;
            var token = JsonConvert.DeserializeObject<TokenResult>(response.Content.ReadAsStringAsync().Result);

            // add access token to request headers
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token?.access_token);
            return client;
        }

        public async Task<HttpResponseMessage> RequestUserByUsernameAsync(string username)
        {
            var client = SetupGraphClient();

            return await client.GetAsync($"https://graph.microsoft.com/v1.0/users?$filter=identities/any(id:id/issuer eq \'TravelTrackApp.onmicrosoft.com\' and id/issuerAssignedId eq \'{username}\')&$select=id,givenName,surname,displayName,identities");
        }

        public async Task<HttpResponseMessage> RequestUserByIdAsync(string id)
        {
            var client = SetupGraphClient();

            return await client.GetAsync($"https://graph.microsoft.com/v1.0/users/{id}?$select=id,givenName,surname,displayName,identities");
        }

        public async Task<HttpResponseMessage> RequestUserIdentitiesAsync()
        {
            var client = SetupGraphClient();

            return await client.GetAsync("https://graph.microsoft.com/v1.0/users?$select=id,givenName,surname,displayName,identities");
        }
    }

}
