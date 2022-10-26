namespace TravelTrack_API_Tests
{
    [TestClass]
    public class UserServiceTests
    {
        private readonly IMapper _mapper;
        private readonly DbContextOptions<TravelTrackContext> _contextOptions;

        public UserServiceTests()
        {
            if (_mapper == null)
            {
                // create automapper config
                var mapConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new UserProfile());
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
        public void UserService_Get_ReturnsListOfAllUsers()
        {
            // Arrange
            var _ctx = NewContext();
            var userService = new UserService(_ctx, _mapper);
            var actualCount = _ctx.Users.Count();

            // Act
            var usersResult = userService.GetAll();

            // Assert 
            Assert.IsInstanceOfType(usersResult, typeof(List<UserDto>));
            Assert.AreEqual(3, usersResult.Count()); // 3 is the number of users seeded in TravelTrackContext class
            Assert.AreEqual("jmoran@ceiamerica.com", usersResult[0].Username);
            Assert.AreEqual("dummyuser@dummy.dum", usersResult[1].Username);
            Assert.AreEqual("fakeuser@fakey.fake", usersResult[2].Username);
        }

        // Tests for Get()

        [TestCategory("Get"), TestCategory("Successful Functionality"), TestMethod]
        public void UserService_Get_ReturnsCorrectUser()
        {
            // Arrange
            var _ctx = NewContext();
            var userService = new UserService(_ctx, _mapper);
            string username = "jmoran@ceiamerica.com";

            // Act
            var userResult = userService.Get(username);

            // Assert 
            Assert.AreEqual(username, userResult.Username);
        }

        [TestCategory("Get"), TestCategory("Not Found"), TestMethod]
        public void UserService_Get_ThrowsNotFoundException()
        {
            // Arrange
            var _ctx = NewContext();
            var userService = new UserService(_ctx, _mapper);
            string username = "nonexistinguser@test.test";


            // Act
            try
            {
                var userResult = userService.Get(username);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                //Assert
                Assert.AreEqual(HttpStatusCode.NotFound, e.Response.StatusCode);
                Assert.AreEqual($"Username Not Found", e.Response.ReasonPhrase);
            }
        }


        // Tests for Add()

        [TestCategory("Add"), TestCategory("Successful Functionality"), TestMethod]
        public void UserService_Add_ReturnsSuccessfullyAddedUser()
        {
            // Arrange
            var _ctx = NewContext();
            var userService = new UserService(_ctx, _mapper);
            var priorUserCount = _ctx.Users.Count();
            var newUser = new UserDto
            {
                Username = "newuser@test.test",
                Password = "P@ssw0rd",
                FirstName = "New", 
                LastName = "User"
            };

            // Act
            var addedUser = userService.Add(newUser);

            // Assert
            Assert.IsNotNull(_ctx.Users.Find(addedUser.Username));
            Assert.AreEqual(priorUserCount + 1, _ctx.Users.Count());
        }

        [TestCategory("Add"), TestCategory("Bad Request"), TestMethod]
        public void UserService_Add_ReturnsBadRequestNullUser()
        {
            // Arrange
            var _ctx = NewContext();
            var userService = new UserService(_ctx, _mapper);
            UserDto newUser = null;

            // Act
            try
            {
                var addedUser = userService.Add(newUser);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Null User", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("Add"), TestCategory("Conflict"), TestMethod]
        public void UserService_Add_ReturnsConflictUserAlreadyExists()
        {
            // Arrange
            var _ctx = NewContext();
            var userService = new UserService(_ctx, _mapper);
            var newUser = new UserDto
            {
                Username = "jmoran@ceiamerica.com",
                Password = "P@ssw0rd",
                FirstName = "Jon",
                LastName = "Moran"
            };

            // Act
            try
            {
                var addedUser = userService.Add(newUser);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.Conflict, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Username already exists in the Database", e.Response.ReasonPhrase);
            }
        }


        // Tests for Delete()

        [TestCategory("Delete"), TestCategory("Successful Functionality"), TestMethod]
        public void UserService_Delete_RemovesUserAndReturnsNothing()
        {
            // Arrange
            var _ctx = NewContext();
            var userService = new UserService(_ctx, _mapper);
            var priorUserCount = _ctx.Users.Count();
            string username = "jmoran@ceiamerica.com";
            var user = _ctx.Users.Find(username);

            // Act
            userService.Delete(username);

            // Assert
            Assert.IsNull(_ctx.Users.Find(username));
            Assert.AreEqual(priorUserCount - 1, _ctx.Users.Count());
        }

        [TestCategory("Delete"), TestCategory("Not Found"), TestMethod]
        public void UserService_Delete_ReturnsNotFoundException()
        {
            // Arrange
            var _ctx = NewContext();
            var userService = new UserService(_ctx, _mapper);
            string username = "nonexistinguser@test.test";

            // Act
            try
            {
                userService.Delete(username);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.NotFound, e.Response.StatusCode);
                Assert.AreEqual("Username Not Found", e.Response.ReasonPhrase);
            }
        }

        // Tests for Update()

        [TestCategory("Update"), TestCategory("Successful Functionality"), TestMethod]
        public void UserService_Update_ReturnsSuccessfullyUpdatedUser()
        {
            // Arrange
            var _ctx = NewContext();
            var userService = new UserService(_ctx, _mapper);
            var priorUserCount = _ctx.Users.Count();
            string username = "dummyuser@dummy.dum";
            var originalUser = userService.Get(username);
            var changedUser = new UserDto
            {
                Username = "dummyuser@dummy.dum",
                Password = "P@ssw0rd",
                FirstName = "Different",
                LastName = "Name"
            };

            // Act
            var userResult = userService.Update(username, changedUser);

            // Assert
            Assert.IsNotNull(_ctx.Users.Find(userResult.Username));
            Assert.AreEqual(changedUser.FirstName, userResult.FirstName);
            Assert.AreNotEqual(userResult.FirstName, originalUser!.FirstName);
        }

        [TestCategory("Update"), TestCategory("Bad Request"), TestMethod]
        public void UserService_Update_ReturnsBadRequestUsernameMismatch()
        {
            // Arrange
            var _ctx = NewContext();
            var userService = new UserService(_ctx, _mapper);
            var priorUserCount = _ctx.Users.Count();
            string username = "dummyuser@dummy.dum";
            var changedUser = new UserDto
            {
                Username = "jmoran@ceiamerica.com",
                Password = "P@ssw0rd",
                FirstName = "Usernames",
                LastName = "Mismatch"
            };

            // Act
            try
            {
                var userResult = userService.Update(username, changedUser);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
                Assert.AreEqual("Bad Request: Username Mismatch", e.Response.ReasonPhrase);
            }
        }

        [TestCategory("Update"), TestCategory("Not Found"), TestMethod]
        public void UserService_Update_ReturnsNotFoundException()
        {
            // Arrange
            var _ctx = NewContext();
            var userService = new UserService(_ctx, _mapper);
            var priorUserCount = _ctx.Users.Count();
            string username = "nonexistinguser@test.test";
            var changedUser = new UserDto
            {
                Username = "nonexistinguser@test.test",
                Password = "P@ssw0rd",
                FirstName = "NotA",
                LastName = "RealUser"
            };

            // Act
            try
            {
                var userResult = userService.Update(username, changedUser);

                // Assert
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                // Assert
                Assert.AreEqual(HttpStatusCode.NotFound, e.Response.StatusCode);
                Assert.AreEqual("Username Not Found", e.Response.ReasonPhrase);
            }
        }
    }
}