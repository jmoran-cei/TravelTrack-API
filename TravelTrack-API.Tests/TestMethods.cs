namespace TravelTrack_API.Tests
{
    class TestMethods
    {
        public static FormFile GetMockedFile(string filePath, string contentType)
        {
            string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName, @"./MockedData/sample-trip-img.jpg");
            using (var stream = File.OpenRead(path))
            {
                return new FormFile(stream, 0, stream.Length, null!, Path.GetFileName(path))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = contentType
                };
            }

        }

        public static IBlobService GetBlobServiceUploadedBlobMock(FormFile mockedFile, string mockReturnedPhotoUrl)
        {
            var mockBlobService = new Mock<IBlobService>();
            mockBlobService.Setup(b => b.UploadPhotoToStorage(mockedFile)).Returns(mockReturnedPhotoUrl);

            return mockBlobService.Object;
        }

        public static IBlobService GetBlobServiceDeleteBlobMock(bool mockReturnedBoolean)
        {
            var mockBlobService = new Mock<IBlobService>();
            mockBlobService.Setup(b => b.Delete(It.IsAny<string>())).Returns(mockReturnedBoolean);

            return mockBlobService.Object;
        }
    }
}
