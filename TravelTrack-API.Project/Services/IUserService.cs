using TravelTrack_API.DTO;

namespace TravelTrack_API.Services
{
    public interface IUserService
    {
        // v1
        List<UserDto> GetAll();
        // v2
        Task<List<UserDto>> GetAllAsync();
        // v3- via Micorosoft Graph API
        Task<List<B2CExistingUserDto>> GetB2CExistingUsersAsync(); 
        // v1
        UserDto Get(string username);
        // v2
        Task<UserDto> GetAsync(string username);
        // v3- via Micorosoft Graph API
        Task<B2CUserDto> GetB2CUserByIdAsync(string Id);
        // v3- via Micorosoft Graph API
        Task<B2CUserDto> GetB2CUserByUsernameAsync(string Id);
        // v1
        UserDto Add(UserDto user);
        // v2
        Task<UserDto> AddAsync(UserDto user);
        // v1
        UserDto Update(string username, UserDto user);
        // v2
        Task<UserDto> UpdateAsync(string username, UserDto user);
        // v1
        void Delete(string username);
        // v2
        Task DeleteAsync(string username);
    }
}
