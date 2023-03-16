using TravelTrack_API.Versions.v2.Models;

namespace TravelTrack_API.Versions.v2.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto> GetAsync(string username);
        Task<UserDto> AddAsync(UserDto user);
        Task<UserDto> UpdateAsync(string username, UserDto user);
        Task DeleteAsync(string username);
    }
}
