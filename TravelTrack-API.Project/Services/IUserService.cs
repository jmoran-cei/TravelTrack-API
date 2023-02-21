using TravelTrack_API.DTO;

namespace TravelTrack_API.Services
{
    public interface IUserService
    {
        List<UserDto> GetAll();
        Task<List<UserDto>> GetAllAsync();
        UserDto Get(string username);
        Task<UserDto> GetAsync(string username);
        UserDto Add(UserDto user);
        Task<UserDto> AddAsync(UserDto user);
        UserDto Update(string username, UserDto user);
        Task<UserDto> UpdateAsync(string username, UserDto user);
        void Delete(string username);
        Task DeleteAsync(string username);
    }
}
