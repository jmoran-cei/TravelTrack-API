namespace TravelTrack_API.Tests
{
    [TestClass]
    public class UserServiceAsyncTests
    {
        private readonly IMapper _mapper;
        private readonly DbContextOptions<TravelTrackContext> _contextOptions;

        public UserServiceAsyncTests()
        {
            if (_mapper == null)
            {
                // create automapper config
                MapperConfiguration mapConfig = new MapperConfiguration(mc =>
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
            using TravelTrackContext ctxToBeCleared = new TravelTrackContext(_contextOptions);
            ctxToBeCleared.Database.EnsureDeleted();

            // set up in memory DB
            using TravelTrackContext ctx = new TravelTrackContext(_contextOptions);
            ctx.Database.EnsureCreated();
        }

        TravelTrackContext NewContext() => new TravelTrackContext(_contextOptions);


        // Tests for GetAll()

        [TestCategory("GetAll"), TestCategory("Successful Functionality"), TestMethod]
        public async Task UserService_Get_ReturnsListOfAllUsers()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            IUserService userService = new UserService(_ctx, _mapper);
            int actualCount = _ctx.Users.Count();

            // Act
            List<UserDto> usersResult = await userService.GetAllAsync();

            // Assert 
            Assert.IsInstanceOfType(usersResult, typeof(List<UserDto>));
            Assert.AreEqual(3, usersResult.Count()); // 3 is the number of users seeded in TravelTrackContext class
            Assert.AreEqual("jmoran@ceiamerica.com", usersResult[0].Username);
            Assert.AreEqual("dummyuser@dummy.dum", usersResult[1].Username);
            Assert.AreEqual("fakeuser@fakey.fake", usersResult[2].Username);
        }

        // Tests for Get()

        [TestCategory("Get"), TestCategory("Successful Functionality"), TestMethod]
        public async Task UserService_Get_ReturnsCorrectUser()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            IUserService userService = new UserService(_ctx, _mapper);
            string username = "jmoran@ceiamerica.com";

            // Act
            UserDto userResult = await userService.GetAsync(username);

            // Assert 
            Assert.AreEqual(username, userResult.Username);
        }

        [TestCategory("Get"), TestCategory("Not Found"), TestMethod]
        public async Task UserService_Get_ThrowsNotFoundException()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            IUserService userService = new UserService(_ctx, _mapper);
            string username = "nonexistinguser@test.test";


            // Act
            try
            {
                UserDto userResult = await userService.GetAsync(username);

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
        public async Task UserService_Add_ReturnsSuccessfullyAddedUser()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            IUserService userService = new UserService(_ctx, _mapper);
            int priorUserCount = _ctx.Users.Count();
            UserDto newUser = new UserDto
            {
                Username = "newuser@test.test",
                Password = "P@ssw0rd",
                FirstName = "New", 
                LastName = "User"
            };

            // Act
            UserDto addedUser = await userService.AddAsync(newUser);

            // Assert
            Assert.IsNotNull(_ctx.Users.Find(addedUser.Username));
            Assert.AreEqual(priorUserCount + 1, _ctx.Users.Count());
        }

        [TestCategory("Add"), TestCategory("Bad Request"), TestMethod]
        public async Task UserService_Add_ReturnsBadRequestNullUser()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            IUserService userService = new UserService(_ctx, _mapper);
            UserDto newUser = null!;

            // Act
            try
            {
                UserDto addedUser = await userService.AddAsync(newUser);

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
        public async Task UserService_Add_ReturnsConflictUserAlreadyExists()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            IUserService userService = new UserService(_ctx, _mapper);
            UserDto newUser = new UserDto
            {
                Username = "jmoran@ceiamerica.com",
                Password = "P@ssw0rd",
                FirstName = "Jon",
                LastName = "Moran"
            };

            // Act
            try
            {
                UserDto addedUser = await userService.AddAsync(newUser);

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
        public async Task UserService_Delete_RemovesUserAndReturnsNothing()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            IUserService userService = new UserService(_ctx, _mapper);
            int priorUserCount = _ctx.Users.Count();
            string username = "jmoran@ceiamerica.com";
            User user = _ctx.Users.Find(username)!;

            // Act
            await userService.DeleteAsync(username);

            // Assert
            Assert.IsNull(_ctx.Users.Find(username));
            Assert.AreEqual(priorUserCount - 1, _ctx.Users.Count());
        }

        [TestCategory("Delete"), TestCategory("Not Found"), TestMethod]
        public async Task UserService_Delete_ReturnsNotFoundException()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            IUserService userService = new UserService(_ctx, _mapper);
            string username = "nonexistinguser@test.test";

            // Act
            try
            {
                await userService.DeleteAsync(username);

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
        public async Task UserService_Update_ReturnsSuccessfullyUpdatedUser()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            IUserService userService = new UserService(_ctx, _mapper);
            int priorUserCount = _ctx.Users.Count();
            string username = "dummyuser@dummy.dum";
            UserDto originalUser = userService.Get(username);
            UserDto changedUser = new UserDto
            {
                Username = "dummyuser@dummy.dum",
                Password = "P@ssw0rd",
                FirstName = "Different",
                LastName = "Name"
            };

            // Act
            UserDto userResult = await userService.UpdateAsync(username, changedUser);

            // Assert
            Assert.IsNotNull(_ctx.Users.Find(userResult.Username));
            Assert.AreEqual(changedUser.FirstName, userResult.FirstName);
            Assert.AreNotEqual(userResult.FirstName, originalUser!.FirstName);
        }

        [TestCategory("Update"), TestCategory("Bad Request"), TestMethod]
        public async Task UserService_Update_ReturnsBadRequestUsernameMismatch()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            IUserService userService = new UserService(_ctx, _mapper);
            int priorUserCount = _ctx.Users.Count();
            string username = "dummyuser@dummy.dum";
            UserDto changedUser = new UserDto
            {
                Username = "jmoran@ceiamerica.com",
                Password = "P@ssw0rd",
                FirstName = "Usernames",
                LastName = "Mismatch"
            };

            // Act
            try
            {
                UserDto userResult = await userService.UpdateAsync(username, changedUser);

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
        public async Task UserService_Update_ReturnsNotFoundException()
        {
            // Arrange
            TravelTrackContext _ctx = NewContext();
            IUserService userService = new UserService(_ctx, _mapper);
            int priorUserCount = _ctx.Users.Count();
            string username = "nonexistinguser@test.test";
            UserDto changedUser = new UserDto
            {
                Username = "nonexistinguser@test.test",
                Password = "P@ssw0rd",
                FirstName = "NotA",
                LastName = "RealUser"
            };

            // Act
            try
            {
                UserDto userResult = await userService.UpdateAsync(username, changedUser);

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