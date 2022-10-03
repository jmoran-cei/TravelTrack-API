using TravelTrack_API.Models;

namespace TravelTrack_API.Services;

public class UserService : IUserService
{
    // Note: I'm using this API for login even though its not best practice; 
    // This is for extra .Net practice while still having a functional login system
    List<User> Users { get; }
    UserService()
    {
        Users = new List<User>
        {
            new User { Username = "jmoran@ceiamerica.com", Password = "P@ssw0rd", FirstName = "Jonathan", LastName = "Moran" },
            new User { Username = "dummyuser@dummy.dum", Password = "P@ssw0rd", FirstName = "Dummy", LastName = "User" },
            new User { Username = "fakeyfake@fakey.fake", Password = "P@ssw0rd", FirstName = "Fake", LastName = "User" },
        };
    }

    public List<User> GetAll() => Users!;
    public User Get(string username) => Users?.FirstOrDefault(u => u.Username == username)!;
    public User Add(User user)
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

    public User Update(User user)
    {
        var index = Users!.FindIndex(u => u.Username == user.Username);
        if (index == -1)
            return user;

        Users[index] = user;
        return user;
    }
}