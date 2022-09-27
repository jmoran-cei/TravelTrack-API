using TravelTrack_API.Models;

namespace TravelTrack_API.Services;

public static class UserService
{
    // Note: I'm using this API for login even though its not best practice; 
    // This is for extra .Net practice while still having a functional login system
    static List<User> Users { get; }
    static UserService()
    {
        Users = new List<User>
        {
            new User { Username = "jmoran@ceiamerica.com", Password = "P@ssw0rd", FirstName = "Jonathan", LastName = "Moran" },
            new User { Username = "dummyuser@dummy.dum", Password = "P@ssw0rd", FirstName = "Dummy", LastName = "User" },
            new User { Username = "fakeyfake@fakey.fake", Password = "P@ssw0rd", FirstName = "Fake", LastName = "User" },
        };
    }

    public static List<User> GetAll() => Users!;
    public static User? Get(string username) => Users?.FirstOrDefault(u => u.Username == username);
    public static void Add(User user)
    {
        Users!.Add(user);
    }

    public static void Delete(string username)
    {
        var user = Get(username);
        if (user is null)
            return;

        Users!.Remove(user);
    }

    public static void Update(User user)
    {
        var index = Users!.FindIndex(u => u.Username == user.Username);
        if (index == -1)
            return;

        Users[index] = user;
    }
}