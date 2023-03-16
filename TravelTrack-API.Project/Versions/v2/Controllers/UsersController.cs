using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using http = System.Web.Http;
using System.Net;
using Microsoft.Identity.Web.Resource;
using TravelTrack_API.Versions.v2.Models;
using TravelTrack_API.Versions.v2.Services;

namespace TravelTrack_API.Versions.v2.Controllers
{
    /// <summary>
    /// Handles incoming user related Http Requests
    /// This version implements all of version 1.0 but asynchronously
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ControllerName("UsersV2")]
    [Route("api/v{version:apiVersion}/[controller]")]
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
        /// Returns all users
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(UserDto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredScope("User.Read")]
        public async Task<ActionResult<List<UserDto>>> GetAllAsync()
        {
            try
            {
                return new OkObjectResult(await _userService.GetAllAsync()); // 200
            }
            catch (Exception e)
            {
                // log to Application Insights
                _logger.LogError(e, e.Message);

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Returns a user when given an existing username
        /// </summary>
        [HttpGet("{username}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredScope("User.Read")]
        public async Task<ActionResult<UserDto>> GetAsync(string username)
        {
            try
            {
                return new OkObjectResult(await _userService.GetAsync(username)); // 200
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
        /// Creates a new user
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredScope("User.Write")]
        public async Task<IActionResult> CreateAsync(UserDto user)
        {
            try
            {
                var addedUser = await _userService.AddAsync(user);
                return new CreatedAtActionResult(nameof(CreateAsync), "Users", new { username = addedUser.Username }, addedUser); // 201
            }
            catch (http.HttpResponseException e)
            {
                if (e.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new NotFoundObjectResult(e.Response); // 404
                }
                if (e.Response.StatusCode == HttpStatusCode.Conflict)
                {
                    return new ConflictObjectResult(e.Response); // 409
                }
                if (e.Response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return new BadRequestObjectResult(e.Response); // 400
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
        /// Updates a user
        /// </summary>
        [HttpPut("{username}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync(string username, UserDto user)
        {
            try
            {
                return new OkObjectResult(await _userService.UpdateAsync(username, user));
            }
            catch (http.HttpResponseException e)
            {
                if (e.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new NotFoundObjectResult(e.Response); // 404
                }
                if (e.Response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return new BadRequestObjectResult(e.Response); // 400
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
        /// Deletes a user
        /// </summary>
        [HttpDelete("{username}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredScope("User.Write")]
        public async Task<IActionResult> DeleteAsync(string username)
        {
            try
            {
                await _userService.DeleteAsync(username);
                return new NoContentResult(); // 204
            }
            catch (http.HttpResponseException e)
            {
                if (e.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new NotFoundObjectResult(e.Response); // 404
                }
                if (e.Response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return new BadRequestObjectResult(e.Response); // 400
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