using System.Net;
using TravelTrack_API.Versions.v2.Models;

namespace TravelTrack_API.Versions.v2.Services
{
    public interface ITripService
    {
        Task<List<TripDto>> GetAllAsync();
        Task<TripDto> GetAsync(long id);
        Task<TripDto> AddAsync(TripDto trip);
        Task<TripDto> UpdateAsync(long id, TripDto trip);
        Task DeleteAsync(long id);
        Task<TripDto> AddPhotoToTripAsync(PhotoDto photo, IFormFile file, long tripId);
        Task<TripDto> RemovePhotosFromTripAsync(List<PhotoDto> photosToRemove, long tripId);
        HttpResponseMessage ResponseMessage(HttpStatusCode statusCode, string content, string reasonPhrase);
    }
}
