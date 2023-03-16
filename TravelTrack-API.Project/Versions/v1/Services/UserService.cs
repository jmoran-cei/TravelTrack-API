using AutoMapper;
using System.Net;
using System.Web.Http;
using TravelTrack_API.Domain;
using TravelTrack_API.DbContexts;
using TravelTrack_API.Versions.v1.Models;

namespace TravelTrack_API.Versions.v1.Services;

public class UserService : IUserService
{
    private readonly TravelTrackContext _ctx;
    private readonly IMapper _mapper;
    public UserService(TravelTrackContext ctx, IMapper mapper)
    {
        _ctx = ctx;
        _mapper = mapper;
    }

    public List<UserDto> GetAll()
    {
        List<User> users = _ctx.Users.ToList();

        List<UserDto> userDTOs = _mapper.Map<List<UserDto>>(users);
        return userDTOs;
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

    private HttpResponseMessage ResponseMessage(HttpStatusCode statusCode, string content, string reasonPhrase)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new ResponseContent.JsonContent(content),
            ReasonPhrase = reasonPhrase
        };
    }
}