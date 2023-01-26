using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace TravelTrack_API.Services.BlobManagement
{
    public class BlobService :IBlobService
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

        public bool Delete(string fileName)
        {
            var container = new BlobContainerClient(_storageConnectionString, "trip-photos");
            return container.GetBlobClient(fileName).DeleteIfExists();
        }

        // DELETE METHOD
    }
}
