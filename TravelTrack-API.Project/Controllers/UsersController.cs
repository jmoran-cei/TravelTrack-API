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

    /// <summary>
    /// UsersController's constructor
    /// </summary>
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Returns all users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(UserDto[]), StatusCodes.Status200OK)]
    public ActionResult<List<UserDto>> GetAll()
    { 
        return new OkObjectResult(_userService.GetAll()); // 200
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
            return new NotFoundObjectResult(e.Response); // 404
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
            return new BadRequestObjectResult(e.Response); // 400
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
            return new BadRequestObjectResult(e.Response); // 400
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
            return new BadRequestObjectResult(e.Response); // 400
        }
    }
}