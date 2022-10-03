using TravelTrack_API.Models;

namespace TravelTrack_API.Services
{
    public interface IUserService
    {
        // unused interface until context, DTOs, and mapper become implemented with EF
        List<User> GetAll();
        User Get(string username);
        User Add(User user);
        User Update(User user);
        void Delete(string username);
    }
}
