namespace TravelTrack_API_Tests
{
    [TestClass]
    public class TripServiceTests
    {
        private readonly IMapper _mapper;
        private readonly DbContextOptions<TravelTrackContext> _contextOptions;

        public TripServiceTests()
        {
            if (_mapper == null)
            {
                // create automapper config
                var mapConfig = new MapperConfiguration(mc =>
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
            using var ctxToBeCleared = new TravelTrackContext(_contextOptions);
            ctxToBeCleared.Database.EnsureDeleted();

            // set up in memory DB
            using var ctx = new TravelTrackContext(_contextOptions);
            ctx.Database.EnsureCreated();
        }

        TravelTrackContext NewContext() => new TravelTrackContext(_contextOptions);


        // Tests for GetAll()

        [TestCategory("GetAll"), TestCategory("Successful Functionality"), TestMethod]
        public void TripService_Get_ReturnsListOfAllTrips()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            var actualCount = _ctx.Trips.Count();

            // Act
            var tripsResult = tripService.GetAll();

            // Assert 
            Assert.IsInstanceOfType(tripsResult, typeof(List<TripDto>));
            Assert.AreEqual(3, tripsResult.Count()); // 3 is the number of trips seeded in TravelTrackContext class
            Assert.AreEqual("Brothers' Anguila Trip", tripsResult[0].Title);
            Assert.AreEqual("Myrtle Beach and Charleston Family Vacay 2022", tripsResult[1].Title);
            Assert.AreEqual("Another Test Trip", tripsResult[2].Title);
        }

        // Tests for Get()

        [TestCategory("Get"), TestCategory("Successful Functionality"), TestMethod]
        public void TripService_Get_ReturnsCorrectTrip()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            long id = 1;

            // Act
            var tripResult = tripService.Get(id);

            // Assert 
            Assert.AreEqual(id, tripResult.Id);
        }

        [TestCategory("Get"), TestCategory("Not Found"), TestMethod]
        public void TripService_Get_ThrowsNotFoundException()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            long id = 9999;


            // Act
            try
            {
                var tripResult = tripService.Get(id);

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


        // Tests for Add()

        [TestCategory("Add"), TestCategory("Successful Functionality"), TestMethod]
        public void TripService_Add_ReturnsSuccessfullyAddedTrip()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            var priorTripCount = _ctx.Trips.Count();
            var priorTripUsersCount = _ctx.TripUsers.Count();
            var priorTripDestinationsCount = _ctx.TripDestinations.Count();
            var priorToDoCount = _ctx.ToDo.Count();
            var newTrip = new TripDto
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
            var addedTrip = tripService.Add(newTrip);

            // Assert
            Assert.IsNotNull(_ctx.Trips.Find(addedTrip.Id));
            Assert.AreEqual(priorTripCount + 1, _ctx.Trips.Count());
            Assert.AreEqual(priorTripUsersCount + newTrip.Members.Count(), _ctx.TripUsers.Count());
            Assert.AreEqual(priorTripDestinationsCount + +newTrip.Destinations.Count(), _ctx.TripDestinations.Count());
            Assert.AreEqual(priorToDoCount + newTrip.ToDo.Count(), _ctx.ToDo.Count());
        }

        [TestCategory("Add"), TestCategory("Bad Request"), TestMethod]
        public void TripService_Add_ReturnsBadRequestNullTrip()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            TripDto newTrip = null;

            // Act
            try
            {
                var addedTrip = tripService.Add(newTrip);

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

        [TestCategory("Add"), TestCategory("Bad Request"), TestMethod]
        public void TripService_Add_ReturnsBadRequestNullTripMembers()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            var newTrip = new TripDto
            {
                Title = "New Trip",
                Details = "Test",
                StartDate = new DateTime(2030, 1, 1),
                EndDate = new DateTime(2030, 2, 1),
                Destinations = new List<DestinationDto>
                {
                    new DestinationDto { Id = "ChIJw4OtEaZjDowRZCw_jCcczqI", City = "Zemi Beach", Region = "West End", Country = "Anguilla" }
                },
                Members = null,
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
                var addedTrip = tripService.Add(newTrip);

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

        [TestCategory("Add"), TestCategory("Bad Request"), TestMethod]
        public void TripService_Add_ReturnsBadRequestNullTripDestinations()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            var newTrip = new TripDto
            {
                Title = "New Trip",
                Details = "Test",
                StartDate = new DateTime(2030, 1, 1),
                EndDate = new DateTime(2030, 2, 1),
                Destinations = null,
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
                var addedTrip = tripService.Add(newTrip);

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

        [TestCategory("Add"), TestCategory("Bad Request"), TestMethod]
        public void TripService_Add_ReturnsBadRequestTripMemberDoesNotExist()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            var newTrip = new TripDto
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
                var addedTrip = tripService.Add(newTrip);

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

        // Tests for Delete()

        [TestCategory("Delete"), TestCategory("Successful Functionality"), TestMethod]
        public void TripService_Delete_RemovesTripAndReturnsNothing()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            var priorTripCount = _ctx.Trips.Count();
            var priorTripUserCount = _ctx.TripUsers.Count();
            var priorTripDestinationCount = _ctx.TripDestinations.Count();
            var priorToDoCount = _ctx.ToDo.Count();
            long id = 1;
            var trip = _ctx.Trips.Find(id);

            // Act
            tripService.Delete(id);

            // Assert
            Assert.IsNull(_ctx.Trips.Find(id));
            Assert.AreEqual(priorTripCount - 1, _ctx.Trips.Count());
            Assert.AreEqual(priorTripUserCount - trip!.Members.Count(), _ctx.TripUsers.Count());
            Assert.AreEqual(priorTripDestinationCount - trip!.Members.Count(), _ctx.Destinations.Count());
            Assert.AreEqual(priorToDoCount - trip!.ToDo.Count(), _ctx.ToDo.Count());
        }

        [TestCategory("Delete"), TestCategory("Not Found"), TestMethod]
        public void TripService_Delete_ReturnsNotFoundException()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            long id = 9999;

            // Act
            try
            {
                tripService.Delete(id);

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

        // Tests for Update()

        [TestCategory("Update"), TestCategory("Successful Functionality"), TestMethod]
        public void TripService_Update_ReturnsSuccessfullyUpdatedTrip()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            var priorTripCount = _ctx.Trips.Count();
            long id = 1;
            var originalTrip = tripService.Get(id);
            var changedTrip = new TripDto
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
            var tripResult = tripService.Update(id, changedTrip);

            // Assert
            Assert.IsNotNull(_ctx.Trips.Find(tripResult.Id));
            Assert.AreEqual(changedTrip.Title, tripResult.Title);
            Assert.AreNotEqual(tripResult.Title, originalTrip!.Title);
        }

        [TestCategory("Update"), TestCategory("Bad Request"), TestMethod]
        public void TripService_Update_ReturnsBadRequestNullTrip()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            var priorTripCount = _ctx.Trips.Count();
            long id = 1;
            TripDto changedTrip = null;


            // Act
            try
            {
                var tripResult = tripService.Update(id, changedTrip);

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

        [TestCategory("Update"), TestCategory("Bad Request"), TestMethod]
        public void TripService_Update_ReturnsBadRequestNullTripDestinations()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            var priorTripCount = _ctx.Trips.Count();
            long id = 1;
            var changedTrip = new TripDto
            {
                Id = 1,
                Title = "Changed Trip",
                Details = "Changed Details",
                StartDate = new DateTime(2030, 1, 1),
                EndDate = new DateTime(2030, 2, 1),
                Destinations = null,
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
                var tripResult = tripService.Update(id, changedTrip);

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

        [TestCategory("Update"), TestCategory("Bad Request"), TestMethod]
        public void TripService_Update_ReturnsBadRequestNullTripMembers()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            var priorTripCount = _ctx.Trips.Count();
            long id = 1;
            var changedTrip = new TripDto
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
                Members = null,
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
                var tripResult = tripService.Update(id, changedTrip);

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

        [TestCategory("Update"), TestCategory("Bad Request"), TestMethod]
        public void TripService_Update_ReturnsBadRequestIdMismatch()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            var priorTripCount = _ctx.Trips.Count();
            long id = 1;
            var changedTrip = new TripDto
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
                var tripResult = tripService.Update(id, changedTrip);

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

        [TestCategory("Update"), TestCategory("Bad Request"), TestMethod]
        public void TripService_Update_ReturnsBadRequestTripMemberDoesNotExist()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            var priorTripCount = _ctx.Trips.Count();
            long id = 1;
            var changedTrip = new TripDto
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
                var tripResult = tripService.Update(id, changedTrip);

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

        [TestCategory("Update"), TestCategory("Not Found"), TestMethod]
        public void TripService_Update_ReturnsNotFoundException()
        {
            // Arrange
            var _ctx = NewContext();
            var tripService = new TripService(_ctx, _mapper);
            var priorTripCount = _ctx.Trips.Count();
            long id = 9999;
            var changedTrip = new TripDto
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
                var tripResult = tripService.Update(id, changedTrip);

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
    }
}