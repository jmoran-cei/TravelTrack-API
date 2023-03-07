using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Web.Http;
using TravelTrack_API.DbContexts;
using TravelTrack_API.Domain;
using TravelTrack_API.DTO;
using TravelTrack_API.MicrosoftGraphModels;
using TravelTrack_API.Services.MicrosoftGraph;

namespace TravelTrack_API.Services;

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

    public List<UserDto> GetAll()
    {
        List<User> users = _ctx.Users.ToList();

        List<UserDto> userDTOs = _mapper.Map<List<UserDto>>(users);
        return userDTOs;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        List<User> users = await _ctx.Users.ToListAsync();

        List<UserDto> userDTOs = _mapper.Map<List<UserDto>>(users);
        return userDTOs;
    }

    /********************************************************
     *  v3 - (1/3) method currently in use 
     ********************************************************/
    public async Task<List<B2CExistingUserDto>> GetB2CExistingUsersAsync()
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

        List<B2CExistingUserDto> existingUsers = new List<B2CExistingUserDto>();

        // map MicrosoftGraphUser to B2CExistingUserDto and add them to existingUsers list
        foreach (MicrosoftGraphUser graphUser in graphUsers!)
        {
            B2CExistingUserDto user = new B2CExistingUserDto();
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

    public UserDto Get(string username)
    {
        var user = _ctx.Users.FirstOrDefault(u => u.Username == username);

        if (user is null)
        {
            throw new HttpResponseException( // 404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No User with Username = {username}",
                    "Username Not Found"
                )
            );
        }

        UserDto userDTOs = _mapper.Map<UserDto>(user);
        return userDTOs;
    }

    public async Task<UserDto> GetAsync(string username)
    {
        var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user is null)
        {
            throw new HttpResponseException( // 404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No User with Username = {username}",
                    "Username Not Found"
                )
            );
        }

        UserDto userDTOs = _mapper.Map<UserDto>(user);
        return userDTOs;
    }

    /********************************************************
     *  v3 - (2/3) methods currently in use 
     ********************************************************/
    public async Task<B2CUserDto> GetB2CUserByIdAsync(string id)
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
        B2CUserDto user = new B2CUserDto
        {
            Id = graphUser.Id,
            Username = getUsernameFromIdentities(graphUser.Identities)!,
            DisplayName = graphUser.DisplayName!,
            FirstName = graphUser.GivenName,
            LastName = graphUser.Surname!,
        };

        return user;
    }

    /********************************************************
    *  v3 - (3/3) methods currently in use 
    ********************************************************/
    public async Task<B2CUserDto> GetB2CUserByUsernameAsync(string username)
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
        string serializedContent = JsonConvert.SerializeObject(content.value[0]);
        // assign to user response content object
        MicrosoftGraphUser? graphUser = JsonConvert.DeserializeObject<MicrosoftGraphUser>(serializedContent);

        // check for miscellaneous error
        if (graphUser is null)
        {
            throw new Exception($"Something went wrong mapping the user data for user with Username={username}");
        }

        // map user from request to B2CUserDto object
        B2CUserDto user = new B2CUserDto
        {
            Id = graphUser.Id,
            Username = getUsernameFromIdentities(graphUser.Identities)!,
            DisplayName = graphUser.DisplayName!,
            FirstName = graphUser.GivenName,
            LastName = graphUser.Surname!,
        };

        return user;
    }

    public UserDto Add(UserDto user)
    {
        if (user is null)
        {
            throw new HttpResponseException( // 400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    "User cannot be null",
                    "Bad Request: Null User"
                )
            );
        }

        if (_ctx.Users.Find(user.Username) is not null)
        {
            throw new HttpResponseException( // 409
                ResponseMessage(
                    HttpStatusCode.Conflict,
                    $"Username = {user.Username} is not unique",
                    "Bad Request: Username already exists in the Database"
                )
            );
        }

        User userEntity = _mapper.Map<User>(user);
        _ctx.Users.Add(userEntity);
        _ctx.SaveChanges();

        return user;
    }

    public async Task<UserDto> AddAsync(UserDto user)
    {
        if (user is null)
        {
            throw new HttpResponseException( // 400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    "User cannot be null",
                    "Bad Request: Null User"
                )
            );
        }

        if (await _ctx.Users.FindAsync(user.Username) is not null)
        {
            throw new HttpResponseException( // 409
                ResponseMessage(
                    HttpStatusCode.Conflict,
                    $"Username = {user.Username} is not unique",
                    "Bad Request: Username already exists in the Database"
                )
            );
        }

        User userEntity = _mapper.Map<User>(user);
        await _ctx.Users.AddAsync(userEntity);
        await _ctx.SaveChangesAsync();

        return user;
    }

    public void Delete(string username)
    {
        var user = _ctx.Users.FirstOrDefault(u => u.Username == username);

        if (user is null)
        {
            throw new HttpResponseException( //404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No User with Username = {username}",
                    "Username Not Found"
                )
            );
        }

        _ctx.Remove(user);
        _ctx.SaveChanges();
    }

    public async Task DeleteAsync(string username)
    {
        var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user is null)
        {
            throw new HttpResponseException( //404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No User with Username = {username}",
                    "Username Not Found"
                )
            );
        }

        _ctx.Remove(user);
        await _ctx.SaveChangesAsync();
    }


    public UserDto Update(string username, UserDto user)
    {
        if (username != user.Username)
        {
            throw new HttpResponseException( //400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    $"Username = {username} and provided User Username = {user.Username} do not match",
                    "Bad Request: Username Mismatch"
                )
            );
        }

        var existingUser = _ctx.Users.FirstOrDefault(u => u.Username == username);

        if (existingUser is null)
        {
            throw new HttpResponseException( // 404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No User with Username = {username}",
                    "Username Not Found"
                )
            );
        }

        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Username = user.Username;
        existingUser.Password = user.Password;

        _ctx.SaveChanges();

        var updatedUser = _mapper.Map<UserDto>(existingUser);
        return updatedUser;
    }

    public async Task<UserDto> UpdateAsync(string username, UserDto user)
    {
        if (username != user.Username)
        {
            throw new HttpResponseException( //400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    $"Username = {username} and provided User Username = {user.Username} do not match",
                    "Bad Request: Username Mismatch"
                )
            );
        }

        var existingUser = await _ctx.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (existingUser is null)
        {
            throw new HttpResponseException( // 404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No User with Username = {username}",
                    "Username Not Found"
                )
            );
        }

        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Username = user.Username;
        existingUser.Password = user.Password;

        await _ctx.SaveChangesAsync();

        var updatedUser = _mapper.Map<UserDto>(existingUser);
        return updatedUser;
    }


    private HttpResponseMessage ResponseMessage(HttpStatusCode statusCode, string content, string reasonPhrase)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new ResponseContent.JsonContent(content),
            ReasonPhrase = reasonPhrase
        };
    }

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
}