using AutoMapper;
using Newtonsoft.Json;
using System.Net;
using System.Web.Http;
using TravelTrack_API.DbContexts;
using TravelTrack_API.Versions.v3.Models;
using TravelTrack_API.MicrosoftGraphModels;
using TravelTrack_API.SharedServices.MicrosoftGraph;

namespace TravelTrack_API.Versions.v3.Services;

public class UserService : IUserService
{
    private readonly TravelTrackContext _ctx;
    private readonly IMapper _mapper;
    private readonly IMicrosoftGraphService _microsoftGraph;
    public UserService(TravelTrackContext ctx, IMapper mapper, IMicrosoftGraphService microsoftGraph)
    {
        _ctx = ctx;
        _mapper = mapper;
        _microsoftGraph = microsoftGraph;
    }


    public async Task<List<MinimalUserDto>> GetB2CExistingUsersAsync()
    {
        // request user from via Microsoft Graph Api
        HttpResponseMessage response = await _microsoftGraph.RequestUserIdentitiesAsync();

        // null check response
        if (response is null)
        {
            throw new Exception("Users' response error with Microsoft Graph");
        }

        ContentOfResponse? content = JsonConvert.DeserializeObject<ContentOfResponse>(response.Content.ReadAsStringAsync().Result);

        // null check content
        if (content is null || content.value is null)
        {
            throw new Exception("Users' repsonse content error with Microsoft Graph");
        }

        string serializedContent = JsonConvert.SerializeObject(content.value); //update: might need changed back to value. take out json prop
        List<MicrosoftGraphUser>? graphUsers = JsonConvert.DeserializeObject<List<MicrosoftGraphUser>>(serializedContent);

        List<MinimalUserDto> existingUsers = new List<MinimalUserDto>();

        // map MicrosoftGraphUser to B2CExistingUserDto and add them to existingUsers list
        foreach (MicrosoftGraphUser graphUser in graphUsers!)
        {
            MinimalUserDto user = new MinimalUserDto();
            // set id
            user.Id = graphUser.Id;
            // parse for username
            string username = getUsernameFromIdentities(graphUser.Identities);

            // add user if user has proper username (is not an B2C AD admin)
            if (username != "")
            {
                // set username
                user.Username = username;
                existingUsers.Add(user);
            }
        }

        return existingUsers;
    }

    public async Task<UserDto> GetB2CUserByIdAsync(string id)
    {
        // request user from via Microsoft Graph Api
        HttpResponseMessage response = await _microsoftGraph.RequestUserByIdAsync(id);

        // check if user exists
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new HttpResponseException( // 404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No User with Id = {id}",
                    "User Id Not Found"
                )
            );
        }

        // assign to user response content object
        MicrosoftGraphUser? graphUser = JsonConvert.DeserializeObject<MicrosoftGraphUser>(response.Content.ReadAsStringAsync().Result);

        // check for miscellaneous error
        if (graphUser is null)
        {
            throw new Exception($"Something went wrong mapping the user data for user with Id={id}");
        }

        // map user from request to B2CUserDto object
        UserDto user = new UserDto
        {
            Id = graphUser.Id,
            Username = getUsernameFromIdentities(graphUser.Identities)!,
            DisplayName = graphUser.DisplayName!,
            FirstName = graphUser.GivenName,
            LastName = graphUser.Surname!,
        };

        return user;
    }

    public async Task<UserDto> GetB2CUserByUsernameAsync(string username)
    {
        // request user from via Microsoft Graph Api
        HttpResponseMessage response = await _microsoftGraph.RequestUserByUsernameAsync(username);

        // null check response
        if (response is null)
        {
            throw new Exception("Users' response error with Microsoft Graph");
        }

        ContentOfResponse? content = JsonConvert.DeserializeObject<ContentOfResponse>(response.Content.ReadAsStringAsync().Result);

        // null check content
        if (content is null)
        {
            throw new Exception("Users' response content error with Microsoft Graph");
        }

        // check if user exists
        if (content.value?.Length == 0)
        {
            throw new HttpResponseException( // 404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No User with Id = {username}",
                    "User Id Not Found"
                )
            );
        }
        // re-serialize for json to graphUser mapping
        string serializedContent = JsonConvert.SerializeObject(content.value?[0]);
        // assign to user response content object
        MicrosoftGraphUser? graphUser = JsonConvert.DeserializeObject<MicrosoftGraphUser>(serializedContent);

        // check for miscellaneous error
        if (graphUser is null)
        {
            throw new Exception($"Something went wrong mapping the user data for user with Username={username}");
        }

        // map user from request to B2CUserDto object
        UserDto user = new UserDto
        {
            Id = graphUser.Id,
            Username = getUsernameFromIdentities(graphUser.Identities)!,
            DisplayName = graphUser.DisplayName!,
            FirstName = graphUser.GivenName,
            LastName = graphUser.Surname!,
        };

        return user;
    }


    // ------- private methods -------
    private string getUsernameFromIdentities(List<MicrosoftGraphUserIdentity> userIdentities)
    {
        foreach (MicrosoftGraphUserIdentity identity in userIdentities)
        {
            // gets username (email) from the correct identity type
            if (identity.SignInType == "emailAddress")
            {
                return identity.IssuerAssignedId!;
            }
        }
        // if signInType is federated, userPrincipalName, etc. then ignore
        return "";
    }


    // ------- public http exception method -------
    private HttpResponseMessage ResponseMessage(HttpStatusCode statusCode, string content, string reasonPhrase)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new ResponseContent.JsonContent(content),
            ReasonPhrase = reasonPhrase
        };
    }

}