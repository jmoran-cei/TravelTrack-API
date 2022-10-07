using TravelTrack_API.DTO;

namespace TravelTrack_API.Services
{
    public interface ITripService
    {
        // unused interface until context, DTOs, and mapper become implemented with EF
        List<Trip> GetAll();
        Trip Get(int id);
        Trip Add(Trip trip);
        Trip Update(Trip trip);
        void Delete(int id);
    }
}
