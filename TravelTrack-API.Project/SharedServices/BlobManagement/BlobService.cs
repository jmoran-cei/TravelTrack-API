using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace TravelTrack_API.SharedServices.BlobManagement
{
    public class BlobService : IBlobService
    {
        private readonly string _storageConnectionString;

        public BlobService(IConfiguration configuration)
        {
            _storageConnectionString = configuration.GetConnectionString("AzureStorage");
        }

        public string Upload(Stream fileStream, string fileName, string contentType)
        {
            var container = new BlobContainerClient(_storageConnectionString, "trip-photos");
            var createResponse = container.CreateIfNotExists();

            if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                container.SetAccessPolicy(PublicAccessType.Blob);

            var blob = container.GetBlobClient(fileName);
            blob.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);

            blob.Upload(fileStream, new BlobHttpHeaders { ContentType = contentType });

            return blob.Uri.ToString();
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
        {
            var container = new BlobContainerClient(_storageConnectionString, "trip-photos");
            var createResponse = await container.CreateIfNotExistsAsync();

            if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                await container.SetAccessPolicyAsync(PublicAccessType.Blob);

            var blob = container.GetBlobClient(fileName);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

            await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });

            return blob.Uri.ToString();
        }

        public bool Delete(string fileName)
        {
            var container = new BlobContainerClient(_storageConnectionString, "trip-photos");
            return container.GetBlobClient(fileName).DeleteIfExists();
        }

        public async Task<bool> DeleteAsync(string fileName)
        {
            var container = new BlobContainerClient(_storageConnectionString, "trip-photos");
            return await container.GetBlobClient(fileName).DeleteIfExistsAsync();
        }

        public string UploadPhotoToStorage(IFormFile file)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return Upload(ms, file.FileName, file.ContentType);
            }
        }

        public async Task<string> UploadPhotoToStorageAsync(IFormFile file)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return await UploadAsync(ms, file.FileName, file.ContentType);
            }
        }
    }
}
