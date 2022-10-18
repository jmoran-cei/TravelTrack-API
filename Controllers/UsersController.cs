using TravelTrack_API.Services;
using TravelTrack_API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

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
    public ActionResult<List<UserDto>> GetAll() =>
        //Ok(_userService.GetAll()); // 200
        Ok(UserServiceTEMP.GetAll()); // 200

    /// <summary>
    /// Returns a user when given an existing username
    /// </summary>
    [HttpGet("{username}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<UserDto> Get(string username)
    {
        //var user = _userService.Get(username);
        var user = UserServiceTEMP.Get(username);

        if (user is null)
            return NotFound(); // 404

        return Ok(user); // 200
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create(UserDto user)
    {
        if (user is null)
            return BadRequest(); // 400

        //_userService.Add(user);
        UserServiceTEMP.Add(user);

        return CreatedAtAction(nameof(Create), "Users", new { username = user.Username }, user); // 201
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
        if (username != user.Username)
            return BadRequest(); // 400

        //var existingUser = _userService.Get(user.Username);
        var existingUser = UserServiceTEMP.Get(user.Username);

        if (existingUser is null)
            return NotFound(); // 404

        //_userService.Update(user);
        UserServiceTEMP.Update(user);

        return Ok(user); // 200
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
        //var user = _userService.Get(username);
        var user = UserServiceTEMP.Get(username);

        if (user is null)
            return NotFound(); // 404

        //_userService.Delete(username);
        UserServiceTEMP.Delete(username);

        return NoContent(); // 204
    }
}