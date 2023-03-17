using TravelTrack_API.Versions.v1.DtoModels;
using v2 = TravelTrack_API.Versions.v2.DtoModels;

namespace TravelTrack_API.SharedServices
{
    public interface IUserService
    {
        /* * * * * * * * * * * * *
         * Version 1
         * * * * * * * * * * * * */
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto> GetAsync(string username);
        Task<UserDto> AddAsync(UserDto user);
        Task<UserDto> UpdateAsync(string username, UserDto user);
        Task DeleteAsync(string username);

        /* * * * * * * * * * * * * * 
         * Version 2: UserDto change, TripDto.Members change
         * 
         * Dto Models changed: UserDto, TripDto.Members
         * - TripDto.Members is mapped with list of UserDtos (new properties for mapping AD B2C User)
         * 
         * Domain Entity changes: 
         * - Added B2CUser entity (similar to User entity, but new properties for AD B2C User data)
         * - Sensitive user data is not stored in this version (only username and id)
         * - Added Trip.B2CMembers entity
         * - Added TripB2CUser join entity for many-to-many
         * - Trip domain entity has new B2CMembers sub-entity for tracking AD B2C User Ids; Trip.Members is still a sub-entity as well to continue v1 support
         * 
         * Methods are coordinated Microsoft Graph API calls for using stored Id/Username to retrieve user data
         * * * * * * * * * * * * * */
        Task<List<v2.MinimalUserDto>> GetB2CExistingUsersAsync();
        Task<v2.UserDto> GetB2CUserByIdAsync(string Id);
        Task<v2.UserDto> GetB2CUserByUsernameAsync(string Username);
    }
}
