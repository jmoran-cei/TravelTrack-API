using TravelTrack_API.DTO;

namespace TravelTrack_API.Services
{
    public interface ITripService
    {
        // unused interface until context, DTOs, and mapper become implemented with EF
        List<TripDto> GetAll();
        TripDto Get(long id);
        TripDto Add(TripDto trip);
        TripDto Update(long id, TripDto trip);
        void Delete(long id);
        TripDto AddPhotoToTrip(PhotoDto photo, IFormFile file, long tripId);
        TripDto RemovePhotosFromTrip(List<PhotoDto> photosToRemove, long tripId);
    }
}
