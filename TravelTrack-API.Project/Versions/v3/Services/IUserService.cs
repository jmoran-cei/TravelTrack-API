using TravelTrack_API.Versions.v3.Models;

namespace TravelTrack_API.Versions.v3.Services
{
    public interface IUserService
    {
        Task<List<MinimalUserDto>> GetB2CExistingUsersAsync();

        Task<UserDto> GetB2CUserByIdAsync(string Id);

        Task<UserDto> GetB2CUserByUsernameAsync(string Username);
    }
}
