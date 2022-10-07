using TravelTrack_API.DTO;

namespace TravelTrack_API.Services;

/* 
 * NOTE: This file is being used in place of UserService.cs TEMPORARILY
 * This file is temporarily being used to deal with static data and allow my UserController to be fully functional for the time being.
 * The UserService.cs will be used w/ it's dependency injection for implementing automapper, context, Domain to DTO conversion
 * This file will be deleted in the near future.
*/
public static class UserServiceTEMP
{
    // Note: I'm using this API for login even though its not best practice; 
    // This is for extra .Net practice while still having a functional login system
    static List<User> Users { get; }
    static UserServiceTEMP()
    {
        Users = new List<User>
        {
            new User { Username = "jmoran@ceiamerica.com", Password = "P@ssw0rd", FirstName = "Jonathan", LastName = "Moran" },
            new User { Username = "dummyuser@dummy.dum", Password = "P@ssw0rd", FirstName = "Dummy", LastName = "User" },
            new User { Username = "fakeyfake@fakey.fake", Password = "P@ssw0rd", FirstName = "Fake", LastName = "User" },
        };
    }

    public static List<User> GetAll() => Users!;
    public static User Get(string username) => Users?.FirstOrDefault(u => u.Username == username)!;
    public static User Add(User user)
    {
        Users!.Add(user);
        return user;
    }

    public static void Delete(string username)
    {
        var user = Get(username);
        if (user is null)
            return;

        Users!.Remove(user);
    }

    public static User Update(User user)
    {
        var index = Users!.FindIndex(u => u.Username == user.Username);
        if (index == -1)
            return user;

        Users[index] = user;
        return user;
    }
}