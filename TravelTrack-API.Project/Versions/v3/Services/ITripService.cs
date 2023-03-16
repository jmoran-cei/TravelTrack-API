using System.Net;
using TravelTrack_API.Versions.v3.Models;

namespace TravelTrack_API.Versions.v3.Services
{
    public interface ITripService
    {
        Task<List<TripDto>> GetTripsByUserIdAsync(string userId);
        Task<TripDto> GetTripB2CAsync(long id);
        Task<TripDto> AddAsync(TripDto trip);
        Task<TripDto> UpdateAsync(long id, TripDto trip);
        Task DeleteAsync(long id);
        Task<TripDto> AddPhotoToB2CTripAsync(PhotoDto photo, IFormFile file, long tripId);
        Task<TripDto> RemovePhotosFromB2CTripAsync(List<PhotoDto> photosToRemove, long tripId);
        HttpResponseMessage ResponseMessage(HttpStatusCode statusCode, string content, string reasonPhrase);
    }
}
