namespace TravelTrack_API.Services.BlobManagement
{
    public interface IBlobService
    {
        string Upload(Stream fileStream, string fileName, string contentType);
        bool Delete(string fileName);
    }
}
