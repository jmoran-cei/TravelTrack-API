using System.Net;
using TravelTrack_API.Versions.v1.DtoModels;
using v2 = TravelTrack_API.Versions.v2.DtoModels;

namespace TravelTrack_API.SharedServices
{
    public interface ITripService
    {
        /* * * * * * * * * * * * *
         * Version 1
         * * * * * * * * * * * * */
        Task<List<TripDto>> GetAllAsync();
        Task<TripDto> GetAsync(long id);
        Task<TripDto> AddAsync(TripDto trip);
        Task<TripDto> UpdateAsync(long id, TripDto trip);
        Task<TripDto> AddPhotoToTripAsync(PhotoDto photo, IFormFile file, long tripId);
        Task<TripDto> RemovePhotosFromTripAsync(List<PhotoDto> photosToRemove, long tripId);

        /* * * * * * * * * * * * * * 
         * Version 2: UserDto change, TripDto.Members change
         * 
         * Dto Models changed: UserDto, TripDto.Members
         * - TripDto.Members is mapped with list of UserDtos (new properties for mapping AD B2C User)
         * 
         * Domain Entity changes: 
         * - Added B2CUser entity (similar to User entity, but new properties for AD B2C User data)
         * - Sensitive user data is not stored in this version (only username and id)
         * - Added Trip.B2CMembers entity for tracking AD B2C User Ids for members of a trip; Trip.Members is still a sub-entity as well to continue v1 support
         * - Added TripB2CUser join entity for many-to-many
         * 
         * Methods all adjusted for Microsoft Graph API calls and mapping TripDto.Members with Trip.B2CMembers instead of Trip.Members
         * * * * * * * * * * * * * */
        Task<List<v2.TripDto>> GetTripsByUserIdAsync(string userId);
        Task<v2.TripDto> GetTripB2CAsync(long id);
        Task<v2.TripDto> AddAsync(v2.TripDto trip);
        Task<v2.TripDto> UpdateAsync(long id, v2.TripDto trip);
        Task<v2.TripDto> AddPhotoToB2CTripAsync(PhotoDto photo, IFormFile file, long tripId);
        Task<v2.TripDto> RemovePhotosFromB2CTripAsync(List<PhotoDto> photosToRemove, long tripId);

        /* * * * * * * * * * * * *
         * Shared between all versions
         * * * * * * * * * * * * */
        Task DeleteAsync(long id);
        HttpResponseMessage ResponseMessage(HttpStatusCode statusCode, string content, string reasonPhrase);
    }
}
