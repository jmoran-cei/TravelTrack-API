namespace TravelTrack_API.Tests
{
    [TestClass]
    public class TripServiceAsyncTests
    {
        private readonly IMapper _mapper;
        private readonly DbContextOptions<TravelTrackContext> _contextOptions;

        public TripServiceAsyncTests()
        {
            if (_mapper == null)
            {
                // create automapper config
                MapperConfiguration mapConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new TripProfile());
                });
                IMapper mapper = mapConfig.CreateMapper();
                _mapper = mapper;
            }

            _contextOptions = new DbContextOptionsBuilder<TravelTrackContext>()
                .UseInMemoryDatabase(databaseName: "TravelTrackTest")
                .Options;

            // wipe in memory DB
            using TravelTrackContext ctxToBeCleared = new TravelTrackContext(_contextOptions);
            ctxToBeCleared.Database.EnsureDeleted();

            // set up in memory DB
            using TravelTrackContext ctx = new TravelTrackContext(_contextOptions);
            ctx.Database.EnsureCreatedAsync();
        }

        TravelTrackContext NewContext() => new TravelTrackContext(_contextOptions);


        // Tests for GetAllAsync()

        [TestCategory("GetAllAsync"), TestCategory("Successful Functionality"), TestMethod]
        public async Task TripService_GetAllAsync_ReturnsListOfAllTrips()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            int actualCount = _ctx.Trips.Count();

            // Act
            List<TripDto> tripsResult = await tripService.GetAllAsync();

            // Assert 
            Assert.IsInstanceOfType(tripsResult, typeof(List<TripDto>));
            Assert.AreEqual(3, tripsResult.Count()); // 3 is the number of trips seeded in TravelTrackContext class
            Assert.AreEqual("Brothers' Anguila Trip", tripsResult[0].Title);
            Assert.AreEqual("Myrtle Beach and Charleston Family Vacay 2022", tripsResult[1].Title);
            Assert.AreEqual("Another Test Trip", tripsResult[2].Title);
        }

        // Tests for GetAsync()

        [TestCategory("GetAsync"), TestCategory("Successful Functionality"), TestMethod]
        public async Task TripService_GetAsync_ReturnsCorrectTrip()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            long id = 1;

            // Act
            TripDto tripResult = await tripService.GetAsync(id);

            // Assert
            Assert.AreEqual(id, tripResult.Id);
        }

        [TestCategory("GetAsync"), TestCategory("Not Found"), TestMethod]
        public async Task TripService_GetAsync_ThrowsNotFoundException()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            long id = 9999;


            // Act
            try
            {
                TripDto tripResult = await tripService.GetAsync(id);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                //Assert
                Assert.AreEqual(HttpStatusCode.NotFound, e.Response.StatusCode);
                Assert.AreEqual("Trip Id Not Found", e.Response.ReasonPhrase);
            }
        }


        // Tests for AddAsync()

        [TestCategory("AddAsync"), TestCategory("Successful Functionality"), TestMethod]
        public async Task TripService_AddAsync_ReturnsSuccessfullyAddedTrip()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            int priorTripCount = _ctx.Trips.Count();
            int priorTripUsersCount = _ctx.TripUsers.Count();
            int priorTripDestinationsCount = _ctx.TripDestinations.Count();
            TripDto newTrip = new TripDto
            {
                Title = "New Trip",
                Details = "Test",
                StartDate = new DateTime(2030, 1, 1),
                EndDate = new DateTime(2030, 2, 1),
                Destinations = new List<DestinationDto>
                {
                    new DestinationDto { Id = "ChIJw4OtEaZjDowRZCw_jCcczqI", City = "Zemi Beach", Region = "West End", Country = "Anguilla" }
                },
                Members = new List<TripUserDto>
                {
                    new TripUserDto { Username = "jmoran@ceiamerica.com", FirstName = "Jonathan", LastName = "Moran" }
                },
                ImgURL = "NewImgUrl.jpg"
            };

            // Act
            TripDto addedTrip = await tripService.AddAsync(newTrip);

            // Assert
            Assert.IsNotNull(_ctx.Trips.Find(addedTrip.Id));
            Assert.AreEqual(priorTripCount + 1, _ctx.Trips.Count());
            Assert.AreEqual(priorTripUsersCount + newTrip.Members.Count(), _ctx.TripUsers.Count());
            Assert.AreEqual(priorTripDestinationsCount + +newTrip.Destinations.Count(), _ctx.TripDestinations.Count());
        }

        [TestCategory("AddAsync"), TestCategory("Bad Request"), TestMethod]
        public async Task TripService_AddAsync_ReturnsBadRequestNullTrip()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            TripDto newTrip = null!;

            // Act
            try
            {
                TripDto addedTrip = await tripService.AddAsync(newTrip);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Null Trip", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("AddAsync"), TestCategory("Bad Request"), TestMethod]
        public async Task TripService_AddAsync_ReturnsBadRequestNullTripMembers()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            TripDto newTrip = new TripDto
            {
                Title = "New Trip",
                Details = "Test",
                StartDate = new DateTime(2030, 1, 1),
                EndDate = new DateTime(2030, 2, 1),
                Destinations = new List<DestinationDto>
                {
                    new DestinationDto { Id = "ChIJw4OtEaZjDowRZCw_jCcczqI", City = "Zemi Beach", Region = "West End", Country = "Anguilla" }
                },
                Members = null!,
                ToDo = new List<ToDoDto>
                {
                    new ToDoDto { Task = "pack clothes", Complete = false },
                    new ToDoDto { Task = "get snacks", Complete = true },
                    new ToDoDto { Task = "finish booking resort", Complete = true },
                    new ToDoDto { Task = "buy more toothpaste", Complete = false },
                },
                ImgURL = "NewImgUrl.jpg"
            };

            // Act
            try
            {
                TripDto addedTrip = await tripService.AddAsync(newTrip);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Null Trip.Members", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("AddAsync"), TestCategory("Bad Request"), TestMethod]
        public async Task TripService_AddAsync_ReturnsBadRequestNullTripDestinations()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            TripDto newTrip = new TripDto
            {
                Title = "New Trip",
                Details = "Test",
                StartDate = new DateTime(2030, 1, 1),
                EndDate = new DateTime(2030, 2, 1),
                Destinations = null!,
                Members = new List<TripUserDto>
                {
                    new TripUserDto { Username = "jmoran@ceiamerica.com", FirstName = "Jonathan", LastName = "Moran" }
                },
                ToDo = new List<ToDoDto>
                {
                    new ToDoDto { Task = "pack clothes", Complete = false },
                    new ToDoDto { Task = "get snacks", Complete = true },
                    new ToDoDto { Task = "finish booking resort", Complete = true },
                    new ToDoDto { Task = "buy more toothpaste", Complete = false },
                },
                ImgURL = "NewImgUrl.jpg"
            };

            // Act
            try
            {
                TripDto addedTrip = await tripService.AddAsync(newTrip);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Null Trip.Destinations", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("AddAsync"), TestCategory("Bad Request"), TestMethod]
        public async Task TripService_AddAsync_ReturnsBadRequestTripMemberDoesNotExist()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            TripDto newTrip = new TripDto
            {
                Title = "New Trip",
                Details = "Test",
                StartDate = new DateTime(2030, 1, 1),
                EndDate = new DateTime(2030, 2, 1),
                Destinations = new List<DestinationDto>
                {
                    new DestinationDto { Id = "ChIJw4OtEaZjDowRZCw_jCcczqI", City = "Zemi Beach", Region = "West End", Country = "Anguilla" }
                },
                Members = new List<TripUserDto>
                {
                    new TripUserDto { Username = "NonExistingUser@test.test", FirstName = "Not", LastName = "Existing" }
                },
                ToDo = new List<ToDoDto>
                {
                    new ToDoDto { Task = "pack clothes", Complete = false },
                    new ToDoDto { Task = "get snacks", Complete = true },
                    new ToDoDto { Task = "finish booking resort", Complete = true },
                    new ToDoDto { Task = "buy more toothpaste", Complete = false },
                },
                ImgURL = "NewImgUrl.jpg"
            };

            // Act
            try
            {
                TripDto addedTrip = await tripService.AddAsync(newTrip);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Trip Member Does Not Exist", e.Response.ReasonPhrase);
            }
        }

        // Tests for DeleteAsync()

        [TestCategory("DeleteAsync"), TestCategory("Successful Functionality"), TestMethod]
        public async Task TripService_DeleteAsync_RemovesTripAndReturnsNothing()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, TestMethods.GetBlobServiceDeleteBlobAsyncMock(true));
            int priorTripCount = _ctx.Trips.Count();
            int priorTripUserCount = _ctx.TripUsers.Count();
            int priorTripDestinationCount = _ctx.TripDestinations.Count();
            int priorToDoCount = _ctx.ToDo.Count();
            int priorPhotosCount = _ctx.Photos.Count();
            long id = 1;
            Trip trip = _ctx.Trips.Find(id);

            // Act
            await tripService.DeleteAsync(id);

            // Assert
            Assert.IsNull(_ctx.Trips.Find(id));
            Assert.AreEqual(priorTripCount - 1, _ctx.Trips.Count());
            Assert.AreEqual(priorTripUserCount - trip!.Members.Count(), _ctx.TripUsers.Count());
            Assert.AreEqual(priorTripDestinationCount - trip!.Members.Count(), _ctx.Destinations.Count());
            Assert.AreEqual(priorToDoCount - trip!.ToDo.Count(), _ctx.ToDo.Count());
            Assert.AreEqual(priorPhotosCount - trip!.Photos.Count(), _ctx.Photos.Count());
        }

        [TestCategory("DeleteAsync"), TestCategory("Not Found"), TestMethod]
        public async Task TripService_DeleteAsync_ReturnsNotFoundException()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            long id = 9999;

            // Act
            try
            {
                await tripService.DeleteAsync(id);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.NotFound, e.Response.StatusCode);
                Assert.AreEqual("Trip Id Not Found", e.Response.ReasonPhrase);
            }
        }

        // Tests for UpdateAsync()

        [TestCategory("UpdateAsync"), TestCategory("Successful Functionality"), TestMethod]
        public async Task TripService_UpdateAsync_ReturnsSuccessfullyUpdatedTrip()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            int priorTripCount = _ctx.Trips.Count();
            long id = 1;
            TripDto originalTrip = await tripService.GetAsync(id);
            TripDto changedTrip = new TripDto
            {
                Id = 1,
                Title = "Changed Trip",
                Details = "Changed Details",
                StartDate = new DateTime(2030, 1, 1),
                EndDate = new DateTime(2030, 2, 1),
                Destinations = new List<DestinationDto>
                {
                    new DestinationDto { Id = "ChIJw4OtEaZjDowRZCw_jCcczqI", City = "Zemi Beach", Region = "West End", Country = "Anguilla" }
                },
                Members = new List<TripUserDto>
                {
                    new TripUserDto { Username = "jmoran@ceiamerica.com", FirstName = "Jonathan", LastName = "Moran" }
                },
                ToDo = new List<ToDoDto>
                {
                    new ToDoDto { Task = "pack clothes", Complete = false },
                    new ToDoDto { Task = "get snacks", Complete = true },
                    new ToDoDto { Task = "finish booking resort", Complete = true },
                    new ToDoDto { Task = "buy more toothpaste", Complete = false }
                },
                ImgURL = "NewImgUrl.jpg"
            };

            // Act
            TripDto tripResult = await tripService.UpdateAsync(id, changedTrip);

            // Assert
            Assert.IsNotNull(_ctx.Trips.Find(tripResult.Id));
            Assert.AreEqual(changedTrip.Title, tripResult.Title);
            Assert.AreNotEqual(tripResult.Title, originalTrip!.Title);
        }

        [TestCategory("UpdateAsync"), TestCategory("Bad Request"), TestMethod]
        public async Task TripService_UpdateAsync_ReturnsBadRequestNullTrip()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            int priorTripCount = _ctx.Trips.Count();
            long id = 1;
            TripDto changedTrip = null!;


            // Act
            try
            {
                TripDto tripResult = await tripService.UpdateAsync(id, changedTrip);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Null Trip", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("UpdateAsync"), TestCategory("Bad Request"), TestMethod]
        public async Task TripService_UpdateAsync_ReturnsBadRequestNullTripDestinations()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            int priorTripCount = _ctx.Trips.Count();
            long id = 1;
            TripDto changedTrip = new TripDto
            {
                Id = 1,
                Title = "Changed Trip",
                Details = "Changed Details",
                StartDate = new DateTime(2030, 1, 1),
                EndDate = new DateTime(2030, 2, 1),
                Destinations = null!,
                Members = new List<TripUserDto>
                {
                    new TripUserDto { Username = "jmoran@ceiamerica.com", FirstName = "Jonathan", LastName = "Moran" }
                },
                ToDo = new List<ToDoDto>
                {
                    new ToDoDto { Task = "pack clothes", Complete = false },
                    new ToDoDto { Task = "get snacks", Complete = true },
                    new ToDoDto { Task = "finish booking resort", Complete = true },
                    new ToDoDto { Task = "buy more toothpaste", Complete = false }
                },
                ImgURL = "NewImgUrl.jpg"
            };

            // Act
            try
            {
                TripDto tripResult = await tripService.UpdateAsync(id, changedTrip);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Null Trip.Destinations", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("UpdateAsync"), TestCategory("Bad Request"), TestMethod]
        public async Task TripService_UpdateAsync_ReturnsBadRequestNullTripMembers()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            int priorTripCount = _ctx.Trips.Count();
            long id = 1;
            TripDto changedTrip = new TripDto
            {
                Id = 1,
                Title = "Changed Trip",
                Details = "Changed Details",
                StartDate = new DateTime(2030, 1, 1),
                EndDate = new DateTime(2030, 2, 1),
                Destinations = new List<DestinationDto>
                {
                    new DestinationDto { Id = "ChIJw4OtEaZjDowRZCw_jCcczqI", City = "Zemi Beach", Region = "West End", Country = "Anguilla" }
                },
                Members = null!,
                ToDo = new List<ToDoDto>
                {
                    new ToDoDto { Task = "pack clothes", Complete = false },
                    new ToDoDto { Task = "get snacks", Complete = true },
                    new ToDoDto { Task = "finish booking resort", Complete = true },
                    new ToDoDto { Task = "buy more toothpaste", Complete = false }
                },
                ImgURL = "NewImgUrl.jpg"
            };

            // Act
            try
            {
                TripDto tripResult = await tripService.UpdateAsync(id, changedTrip);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Null Trip.Members", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("UpdateAsync"), TestCategory("Bad Request"), TestMethod]
        public async Task TripService_UpdateAsync_ReturnsBadRequestIdMismatch()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            int priorTripCount = _ctx.Trips.Count();
            long id = 1;
            TripDto changedTrip = new TripDto
            {
                Id = 2,
                Title = "Changed Trip",
                Details = "Changed Details",
                StartDate = new DateTime(2030, 1, 1),
                EndDate = new DateTime(2030, 2, 1),
                Destinations = new List<DestinationDto>
                {
                    new DestinationDto { Id = "ChIJw4OtEaZjDowRZCw_jCcczqI", City = "Zemi Beach", Region = "West End", Country = "Anguilla" }
                },
                Members = new List<TripUserDto>
                {
                    new TripUserDto { Username = "jmoran@ceiamerica.com", FirstName = "Jonathan", LastName = "Moran" }
                },
                ToDo = new List<ToDoDto>
                {
                    new ToDoDto { Task = "pack clothes", Complete = false },
                    new ToDoDto { Task = "get snacks", Complete = true },
                    new ToDoDto { Task = "finish booking resort", Complete = true },
                    new ToDoDto { Task = "buy more toothpaste", Complete = false }
                },
                ImgURL = "NewImgUrl.jpg"
            };

            // Act
            try
            {
                TripDto tripResult = await tripService.UpdateAsync(id, changedTrip);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Id Mismatch", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("UpdateAsync"), TestCategory("Bad Request"), TestMethod]
        public async Task TripService_UpdateAsync_ReturnsBadRequestTripMemberDoesNotExist()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            int priorTripCount = _ctx.Trips.Count();
            long id = 1;
            TripDto changedTrip = new TripDto
            {
                Id = 1,
                Title = "Changed Trip",
                Details = "Changed Details",
                StartDate = new DateTime(2030, 1, 1),
                EndDate = new DateTime(2030, 2, 1),
                Destinations = new List<DestinationDto>
                {
                    new DestinationDto { Id = "ChIJw4OtEaZjDowRZCw_jCcczqI", City = "Zemi Beach", Region = "West End", Country = "Anguilla" }
                },
                Members = new List<TripUserDto>
                {
                    new TripUserDto { Username = "NonExistingUser@test.test", FirstName = "Not", LastName = "Existing" }
                },
                ToDo = new List<ToDoDto>
                {
                    new ToDoDto { Task = "pack clothes", Complete = false },
                    new ToDoDto { Task = "get snacks", Complete = true },
                    new ToDoDto { Task = "finish booking resort", Complete = true },
                    new ToDoDto { Task = "buy more toothpaste", Complete = false }
                },
                ImgURL = "NewImgUrl.jpg"
            };

            // Act
            try
            {
                TripDto tripResult = await tripService.UpdateAsync(id, changedTrip);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Trip Member Does Not Exist", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("UpdateAsync"), TestCategory("Not Found"), TestMethod]
        public async Task TripService_UpdateAsync_ReturnsNotFoundException()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            ITripService tripService = new TripService(_ctx, _mapper, null!);
            int priorTripCount = _ctx.Trips.Count();
            long id = 9999;
            TripDto changedTrip = new TripDto
            {
                Id = 9999,
                Title = "Changed Trip",
                Details = "Changed Details",
                StartDate = new DateTime(2030, 1, 1),
                EndDate = new DateTime(2030, 2, 1),
                Destinations = new List<DestinationDto>
                {
                    new DestinationDto { Id = "ChIJw4OtEaZjDowRZCw_jCcczqI", City = "Zemi Beach", Region = "West End", Country = "Anguilla" }
                },
                Members = new List<TripUserDto>
                {
                    new TripUserDto { Username = "NonExistingUser@test.test", FirstName = "Not", LastName = "Existing" }
                },
                ToDo = new List<ToDoDto>
                {
                    new ToDoDto { Task = "pack clothes", Complete = false },
                    new ToDoDto { Task = "get snacks", Complete = true },
                    new ToDoDto { Task = "finish booking resort", Complete = true },
                    new ToDoDto { Task = "buy more toothpaste", Complete = false }
                },
                ImgURL = "NewImgUrl.jpg"
            };

            // Act
            try
            {
                TripDto tripResult = await tripService.UpdateAsync(id, changedTrip);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.NotFound, e.Response.StatusCode);
                Assert.AreEqual("Trip Id Not Found", e.Response.ReasonPhrase);
            }
        }


        
        // Tests for AddPhotoToTripAsync()

        [TestCategory("AddPhotoToTripAsync"), TestCategory("Successful Functionality"), TestMethod]
        public async Task TripService_AddPhotoToTripAsync_ReturnsSuccessfullyUpdatedTrip()
        {
            // --- Arrange --- 
            TravelTrackContext _ctx = NewContext();
            PhotoDto newTripPhoto = new PhotoDto
            {
                TripId = 3,
                FileName = "3-sample-trip-img.jpg",
                AddedByUser = "jmoran@ceiamerica.com",
                Path = "",
                Alt = "sample-trip-img.jpg",
                FileType = "image/jpg"
            };
            FormFile mockedFile = TestMethods.GetMockedFile(@"./MockedData/sample-trip-img.jpg", "image/jpeg");
            string mockReturnedPhotoUrl = "https://fakestorageaccount.blob.core.windows.net/fakecontainer/2-sample-trip-img.jpg";
            long tripId = 3;

            ITripService tripService = new TripService(_ctx, _mapper, TestMethods.GetBlobServiceUploadedBlobAsyncMock(mockedFile!, mockReturnedPhotoUrl));
            int priorPhotosCount = _ctx.Photos.Count();
            Trip priorVersionOfTrip = _ctx.Trips.FirstOrDefault(t => t.Id == tripId)!;
            int priorTripPhotoCount = priorVersionOfTrip.Photos.Count();


            // --- Act --- 
            TripDto updatedTrip = await tripService.AddPhotoToTripAsync(newTripPhoto, mockedFile, tripId);


            // --- Assert --- 
            // check that trip has the photo added to it
            Assert.IsNotNull(updatedTrip.Photos.FirstOrDefault(p => p.FileName == newTripPhoto.FileName));
            // check that photo was added to DB
            Assert.IsNotNull(_ctx.Photos.Where(p => p.FileName == newTripPhoto.FileName));
            // check that photo path was generated and set within the new photo of the updated trip
            Assert.AreEqual(mockReturnedPhotoUrl, updatedTrip.Photos.FirstOrDefault(p => p.FileName == newTripPhoto.FileName)!.Path);
        }

        [TestCategory("AddPhotoToTripAsync"), TestCategory("Bad Request"), TestMethod]
        public async Task TripService_AddPhotoToTripAsync_ReturnsNullPhoto()
        {
            // --- Arrange --- 
            TravelTrackContext _ctx = NewContext();
            long tripId = 1;
            PhotoDto newTripPhoto = null!;
            FormFile mockPhotoFile = new FormFile(new MemoryStream(), 0, 0, "", "");
            ITripService tripService = new TripService(_ctx, _mapper, null!);

            // --- Act --- 
            try
            {
                await tripService.AddPhotoToTripAsync(newTripPhoto, mockPhotoFile, tripId);

                //  --- Assert --- 
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Null Photo", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("AddPhotoToTripAsync"), TestCategory("Bad Request"), TestMethod]
        public async Task TripService_AddPhotoToTripAsync_ReturnsBadRequestInvalidFileType()
        {
            // --- Arrange --- 
            TravelTrackContext _ctx = NewContext();
            long tripId = 2;
            PhotoDto newTripPhoto = new PhotoDto
            {
                TripId = 2,
                FileName = "2-invalid-file.pdf",
                AddedByUser = "jmoran@ceiamerica.com",
                Path = "",
                Alt = "invalid-file.pdf",
                FileType = "application/pdf"
            };
            FormFile mockedFile = TestMethods.GetMockedFile(@"./MockedData/sample-trip-img.jpg", "application/pdf");

            ITripService tripService = new TripService(_ctx, _mapper, null!);

            // --- Act --- 
            try
            {
                await tripService.AddPhotoToTripAsync(newTripPhoto, mockedFile, tripId);

                //  --- Assert --- 
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Invalid File Type", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("AddPhotoToTripAsync"), TestCategory("Bad Request"), TestMethod]
        public async Task TripService_AddPhotoToTripAsync_ReturnsBadRequestIdMismatch()
        {
            // --- Arrange --- 
            TravelTrackContext _ctx = NewContext();
            long tripId = 3;
            PhotoDto newTripPhoto = new PhotoDto
            {
                TripId = 2,
                FileName = "sample-trip-img.jpg",
                AddedByUser = "jmoran@ceiamerica.com",
                Path = "",
                Alt = "sample-trip-img.jpg",
                FileType = "image/jpeg"
            };
            FormFile mockedFile = TestMethods.GetMockedFile(@"./MockedData/sample-trip-img.jpg", "image/jpeg");
            ITripService tripService = new TripService(_ctx, _mapper, null!);

            // --- Act --- 
            try
            {
                await tripService.AddPhotoToTripAsync(newTripPhoto, mockedFile, tripId);

                //  --- Assert --- 
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Id Mismatch", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("AddPhotoToTripAsync"), TestCategory("Not Found"), TestMethod]
        public async Task TripService_AddPhotoToTripAsync_ReturnsNotFoundException()
        {
            // --- Arrange --- 
            TravelTrackContext _ctx = NewContext();
            long tripId = -9999;
            PhotoDto newTripPhoto = new PhotoDto
            {
                TripId = -9999,
                FileName = "sample-trip-img.jpg",
                AddedByUser = "jmoran@ceiamerica.com",
                Path = "",
                Alt = "sample-trip-img.jpg",
                FileType = "image/jpeg"
            };
            FormFile mockedFile = TestMethods.GetMockedFile(@"./MockedData/sample-trip-img.jpg", "image/jpeg");
            ITripService tripService = new TripService(_ctx, _mapper, null!);

            // --- Act --- 
            try
            {
                await tripService.AddPhotoToTripAsync(newTripPhoto, mockedFile, tripId);

                //  --- Assert --- 
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, e.Response.StatusCode);
                Assert.AreEqual("Trip Not Found", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("AddPhotoToTripAsync"), TestCategory("Bad Request"), TestMethod]
        public async Task TripService_AddPhotoToTripAsync_ReturnsConflictException()
        {
            // --- Arrange --- 
            TravelTrackContext _ctx = NewContext();
            long tripId = 2;
            PhotoDto newTripPhoto = new PhotoDto
            {
                TripId = 2,
                FileName = "2-sample-trip-img.jpg",
                AddedByUser = "jmoran@ceiamerica.com",
                Path = "",
                Alt = "sample-trip-img.jpg",
                FileType = "image/jpg"
            };
            FormFile mockedFile = TestMethods.GetMockedFile(@"./MockedData/sample-trip-img.jpg", "image/jpeg");
            string mockReturnedPhotoUrl = "https://fakestorageaccount.blob.core.windows.net/fakecontainer/2-sample-trip-img.jpg";

            ITripService tripService = new TripService(_ctx, _mapper, TestMethods.GetBlobServiceUploadedBlobAsyncMock(mockedFile, mockReturnedPhotoUrl));
            int priorPhotosCount = _ctx.Photos.Count();
            Trip priorVersionOfTrip = _ctx.Trips.FirstOrDefault(t => t.Id == tripId)!;
            int priorTripPhotoCount = priorVersionOfTrip.Photos.Count();


            // --- Act --- 
            try
            {
                // add photo successfully
                await tripService.AddPhotoToTripAsync(newTripPhoto, mockedFile, tripId);
                // try adding the same photo again
                await tripService.AddPhotoToTripAsync(newTripPhoto, mockedFile, tripId);

                //  --- Assert --- 
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                Assert.AreEqual(HttpStatusCode.Conflict, e.Response.StatusCode);
                Assert.AreEqual($"'{newTripPhoto.Alt}' already exists for this trip.", e.Response.ReasonPhrase);
            }
        }



        // Tests for RemovePhotosFromTripAsync()

        [TestCategory("RemovePhotosFromTripAsync"), TestCategory("Successful Functionality"), TestMethod]
        public async Task TripService_RemovePhotosFromTripAsync_ReturnsSuccessfullyUpdatedTrip()
        {
            // --- Arrange --- 
            TravelTrackContext _ctx = NewContext();
            List<PhotoDto> photosToRemove = new List<PhotoDto> {
                new PhotoDto
                {
                    Id = _ctx.Photos.Where(p => p.FileName == "1-sample-trip-img.jpg").SingleOrDefault()!.Id,
                    TripId = 1,
                    FileName = "1-sample-trip-img.jpg",
                    AddedByUser = "jmoran@ceiamerica.com",
                    Path = "",
                    Alt = "1-sample-trip-img.jpg",
                    FileType = "image/jpg"
                 },
                new PhotoDto
                {
                    Id = _ctx.Photos.Where(p => p.FileName == "1-travel-track-readme-img.jpg").SingleOrDefault()!.Id,
                    TripId = 1,
                    FileName = "1-travel-track-readme-img.jpg",
                    AddedByUser = "jmoran@ceiamerica.com",
                    Path = "",
                    Alt = "1-travel-track-readme-img.jpg",
                    FileType = "image/jpg"
                }
            };
            long tripId = 1;

            ITripService tripService = new TripService(_ctx, _mapper, TestMethods.GetBlobServiceDeleteBlobAsyncMock(true));
            int priorPhotosCount = _ctx.Photos.Count();
            Trip priorVersionOfTrip = _ctx.Trips.FirstOrDefault(t => t.Id == tripId)!;
            int priorTripPhotoCount = priorVersionOfTrip.Photos.Count();


            // --- Act --- 
            TripDto updatedTrip = await tripService.RemovePhotosFromTripAsync(photosToRemove, tripId);


            // --- Assert ---
            // check that photos were removed from DB
            Assert.IsNull(_ctx.Photos.FirstOrDefault(p => p.Id == photosToRemove[0].Id));
            Assert.IsNull(_ctx.Photos.FirstOrDefault(p => p.Id == photosToRemove[1].Id));
            // check that trip has the photos removed from it
            Assert.AreEqual(priorTripPhotoCount - photosToRemove.Count(), updatedTrip.Photos.Count());
            // check that photos no longer exist in the updated trip
            Assert.IsNull(updatedTrip.Photos.FirstOrDefault(p => p.Id == photosToRemove[0].Id));
            Assert.IsNull(updatedTrip.Photos.FirstOrDefault(p => p.Id == photosToRemove[1].Id));
        }

        [TestCategory("RemovePhotosFromTripAsync"), TestCategory("Bad Request"), TestMethod]
        public async Task TripService_RemovePhotosFromTripAsync_ReturnsBadRequestNullPhotos()
        {
            // --- Arrange --- 
            TravelTrackContext _ctx = NewContext();
            List<PhotoDto> photosToRemove = null!;
            long tripId = 2;
            ITripService tripService = new TripService(_ctx, _mapper, null!);

            // --- Act --- 
            try
            {
                TripDto updatedTrip = await tripService.RemovePhotosFromTripAsync(photosToRemove, tripId);

                // --- Assert ---
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Null Photos", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("RemovePhotosFromTripAsync"), TestCategory("Bad Request"), TestMethod]
        public async Task TripService_RemovePhotosFromTripAsync_ReturnsBadRequestIdMismatch()
        {
            // --- Arrange --- 
            TravelTrackContext _ctx = NewContext();
            List<PhotoDto> photosToRemove = new List<PhotoDto> {
                new PhotoDto
                {
                    Id = _ctx.Photos.Where(p => p.FileName == "1-sample-trip-img.jpg").SingleOrDefault()!.Id,
                    TripId = 1,
                    FileName = "1-sample-trip-img.jpg",
                    AddedByUser = "jmoran@ceiamerica.com",
                    Path = "",
                    Alt = "1-sample-trip-img.jpg",
                    FileType = "image/jpg"
                 },
                new PhotoDto
                {
                    Id = _ctx.Photos.Where(p => p.FileName == "1-travel-track-readme-img.jpg").SingleOrDefault()!.Id,
                    TripId = 1,
                    FileName = "1-travel-track-readme-img.jpg",
                    AddedByUser = "jmoran@ceiamerica.com",
                    Path = "",
                    Alt = "1-travel-track-readme-img.jpg",
                    FileType = "image/jpg"
                }
            };
            long tripId = 3;
            ITripService tripService = new TripService(_ctx, _mapper, null!);

            // --- Act --- 
            try
            {
                TripDto updatedTrip = await tripService.RemovePhotosFromTripAsync(photosToRemove, tripId);

                // --- Assert ---
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Id Mismatch", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("RemovePhotosFromTripAsync"), TestCategory("Not Found"), TestMethod]
        public async Task TripService_RemovePhotosFromTripAsync_ReturnsNotFoundException()
        {
            // --- Arrange --- 
            TravelTrackContext _ctx = NewContext();
            List<PhotoDto> photosToRemove = new List<PhotoDto> {
                new PhotoDto
                {
                    Id = _ctx.Photos.Where(p => p.FileName == "1-sample-trip-img.jpg").SingleOrDefault()!.Id,
                    TripId = -9999,
                    FileName = "1-sample-trip-img.jpg",
                    AddedByUser = "jmoran@ceiamerica.com",
                    Path = "",
                    Alt = "1-sample-trip-img.jpg",
                    FileType = "image/jpg"
                 },
                new PhotoDto
                {
                    Id = _ctx.Photos.Where(p => p.FileName == "1-travel-track-readme-img.jpg").SingleOrDefault()!.Id,
                    TripId = -9999,
                    FileName = "1-travel-track-readme-img.jpg",
                    AddedByUser = "jmoran@ceiamerica.com",
                    Path = "",
                    Alt = "1-travel-track-readme-img.jpg",
                    FileType = "image/jpg"
                }
            };
            long tripId = -9999;
            ITripService tripService = new TripService(_ctx, _mapper, null!);

            // --- Act --- 
            try
            {
                TripDto updatedTrip = await tripService.RemovePhotosFromTripAsync(photosToRemove, tripId);

                // --- Assert ---
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, e.Response.StatusCode);
                Assert.AreEqual("Trip Not Found", e.Response.ReasonPhrase);
            }
        }
    }
}
