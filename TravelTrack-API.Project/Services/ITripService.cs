using TravelTrack_API.DTO;

namespace TravelTrack_API.Services
{
    public interface ITripService
    {
        List<TripDto> GetAll();
        Task<List<TripDto>> GetAllAsync();
        TripDto Get(long id);
        Task<TripDto> GetAsync(long id);
        TripDto Add(TripDto trip);
        Task<TripDto> AddAsync(TripDto trip);
        TripDto Update(long id, TripDto trip);
        Task<TripDto> UpdateAsync(long id, TripDto trip);
        void Delete(long id);
        Task DeleteAsync(long id);
        TripDto AddPhotoToTrip(PhotoDto photo, IFormFile file, long tripId);
        Task<TripDto> AddPhotoToTripAsync(PhotoDto photo, IFormFile file, long tripId);
        TripDto RemovePhotosFromTrip(List<PhotoDto> photosToRemove, long tripId);
        Task<TripDto> RemovePhotosFromTripAsync(List<PhotoDto> photosToRemove, long tripId);
    }
}
