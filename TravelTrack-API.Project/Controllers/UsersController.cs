using TravelTrack_API.Services;
using TravelTrack_API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using http = System.Web.Http;
using System.Net;

namespace Users.Controllers;

/// <summary>
/// Handles incoming user related Http Requests
/// </summary>
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
[Route("api/[controller]")]
[EnableCors()]
[ApiVersion("1.0")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    /// <summary>
    /// UsersController's constructor
    /// </summary>
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
    public ActionResult<List<UserDto>> GetAll()
    { 
        try
        {
            return new OkObjectResult(_userService.GetAll()); // 200
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<UserDto> Get(string username)
    {
        try
        {
            return new OkObjectResult(_userService.Get(username)); // 200
        }
        catch (http.HttpResponseException e)
        {
            if (e.Response.StatusCode == HttpStatusCode.NotFound)
            {
                return new NotFoundObjectResult(e.Response); // 404
            }
            // log to Application Insights
            _logger.LogError(e, e.Response.ReasonPhrase);

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
    public IActionResult Create(UserDto user)
    {
        try
        {
            var addedUser = _userService.Add(user);
            return new CreatedAtActionResult(nameof(Create), "Users", new { username = addedUser.Username }, addedUser); // 201
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
            _logger.LogError(e, e.Response.ReasonPhrase);

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
    public IActionResult Update(string username, UserDto user)
    {
        try
        {
            return new OkObjectResult(_userService.Update(username, user));
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
            _logger.LogError(e, e.Response.ReasonPhrase);

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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(string username)
    {
        try
        {
            _userService.Delete(username);
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
            _logger.LogError(e, e.Response.ReasonPhrase);

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