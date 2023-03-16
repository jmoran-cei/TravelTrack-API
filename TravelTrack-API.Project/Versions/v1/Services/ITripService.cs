using System.Net;
using TravelTrack_API.Versions.v1.Models;

namespace TravelTrack_API.Versions.v1.Services
{
    public interface ITripService
    {
        List<TripDto> GetAll();

        TripDto Get(long id);

        TripDto Add(TripDto trip);

        TripDto Update(long id, TripDto trip);

        void Delete(long id);

        TripDto AddPhotoToTrip(PhotoDto photo, IFormFile file, long tripId);

        TripDto RemovePhotosFromTrip(List<PhotoDto> photosToRemove, long tripId);

        HttpResponseMessage ResponseMessage(HttpStatusCode statusCode, string content, string reasonPhrase);
    }
}
