using TravelTrack_API.Versions.v1.Models;

namespace TravelTrack_API.Versions.v1.Services
{
    public interface IUserService
    {
        List<UserDto> GetAll();
        
        UserDto Get(string username);

        UserDto Add(UserDto user);
        
        UserDto Update(string username, UserDto user);
        void Delete(string username);
    }
}
