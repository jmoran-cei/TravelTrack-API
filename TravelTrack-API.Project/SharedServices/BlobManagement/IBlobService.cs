﻿namespace TravelTrack_API.SharedServices.BlobManagement
{
    public interface IBlobService
    {
        string Upload(Stream fileStream, string fileName, string contentType);
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
        bool Delete(string fileName);
        Task<bool> DeleteAsync(string fileName);
        string UploadPhotoToStorage(IFormFile file);
        Task<string> UploadPhotoToStorageAsync(IFormFile file);
    }
}
