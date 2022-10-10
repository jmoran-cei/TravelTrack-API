using TravelTrack_API.DTO;

namespace TravelTrack_API.Services;

public class UserService: IUserService
{
    // Note: I'm using this API for login even though its not best practice; 
    // This is for extra .Net practice while still having a functional login system
    List<UserDto> Users { get; }
    public UserService()
    {
        Users = new List<UserDto>
        {
            new UserDto { Username = "jmoran@ceiamerica.com", Password = "P@ssw0rd", FirstName = "Jonathan", LastName = "Moran" },
            new UserDto { Username = "dummyuser@dummy.dum", Password = "P@ssw0rd", FirstName = "Dummy", LastName = "User" },
            new UserDto { Username = "fakeyfake@fakey.fake", Password = "P@ssw0rd", FirstName = "Fake", LastName = "User" },
        };
    }

    public List<UserDto> GetAll() => Users!;
    public UserDto Get(string username) => Users?.FirstOrDefault(u => u.Username == username)!;
    public UserDto Add(UserDto user)
    {
        Users!.Add(user);
        return user;
    }

    public void Delete(string username)
    {
        var user = Get(username);
        if (user is null)
            return;

        Users!.Remove(user);
    }

    public UserDto Update(string username, UserDto user)
    {
        var index = Users!.FindIndex(u => u.Username == user.Username);
        if (index == -1)
            return user;

        Users[index] = user;
        return user;
    }
}