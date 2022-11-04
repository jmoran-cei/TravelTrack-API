using TravelTrack_API.DTO;

namespace TravelTrack_API.Services
{
    public interface IUserService
    {
        // unused interface until context, DTOs, and mapper become implemented with EF
        List<UserDto> GetAll();
        UserDto Get(string username);
        UserDto Add(UserDto user);
        UserDto Update(string username, UserDto user);
        void Delete(string username);
    }
}
