using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using http = System.Web.Http;
using System.Net;
using Microsoft.Identity.Web.Resource;
using TravelTrack_API.Versions.v2.DtoModels;
using TravelTrack_API.SharedServices;

namespace TravelTrack_API.Versions.v2.Controllers
{
    /// <summary>
    /// Handles incoming user related Http Requests
    /// This version implements the Azure Directory B2C connection to the API and appropriate changes needed to get user data
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ControllerName("UsersV2")]
    [Route("api/[controller]")]
    [Route("api/[controller]/v{version:apiVersion}")]
    [EnableCors()]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        
        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        /// <summary>
        /// Returns all usernames and AD B2C object Ids
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<MinimalUserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredScope("User.Read")]
        public async Task<ActionResult<List<UserDto>>> GetB2CExistingUsersAsync()
        {
            try
            {
                // gets usernames and ids for all users
                return new OkObjectResult(await _userService.GetB2CExistingUsersAsync()); // 200
            }
            catch (Exception e)
            {
                // log to Application Insights
                _logger.LogError(e, e.Message);

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Gets user when given an existing user id
        /// </summary>
        [HttpGet("id/{userId}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredScope("User.Read")]
        public async Task<ActionResult<UserDto>> GetB2CUserByIdAsync(string userId)
        {
            try
            {
                return new OkObjectResult(await _userService.GetB2CUserByIdAsync(userId));
            }
            catch (http.HttpResponseException e)
            {
                if (e.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new NotFoundObjectResult(e.Response); // 404
                }
                // log to Application Insights
                _logger.LogError(e, e.Response.ToString());

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            catch (Exception e)
            {
                // log to Application Insights
                _logger.LogError(e, e.Message);

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Returns a AD B2C user when provided an existing
        /// </summary>
        [HttpGet("{username}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredScope("User.Read")]
        public async Task<ActionResult<UserDto>> GetB2CUserByUsernameAsync(string username)
        {
            try
            {
                // gets B2C user object from Microsoft Graph
                return new OkObjectResult(await _userService.GetB2CUserByUsernameAsync(username));
            }
            catch (http.HttpResponseException e)
            {
                if (e.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new NotFoundObjectResult(e.Response); // 404
                }
                // log to Application Insights
                _logger.LogError(e, e.Response.ToString());

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            catch (Exception e)
            {
                // log to Application Insights
                _logger.LogError(e, e.Message);

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}