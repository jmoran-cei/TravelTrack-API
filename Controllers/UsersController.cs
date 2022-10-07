using TravelTrack_API.Services;
using TravelTrack_API.Models;
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
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// UsersController's constructor
    /// </summary>
    public UsersController(IUserService userService) {
        _userService = userService;
    }

    /// <summary>
    /// Returns all users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(User[]), StatusCodes.Status200OK)]
    public ActionResult<List<User>> GetAll() =>
        Ok(_userService.GetAll()); // 200

    /// <summary>
    /// Returns a user when given an existing username
    /// </summary>
    [HttpGet("{username}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<User> Get(string username)
    {
        var user = _userService.Get(username);

        if (user is null)
            return NotFound(); // 404

        return Ok(user); // 200
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create(User user)
    {
        if (user is null)
            return BadRequest(); // 400

        _userService.Add(user);

        return CreatedAtAction(nameof(Create), "Users", new { username = user.Username }, user); // 201
    }

    /// <summary>
    /// Updates a user
    /// </summary>
    [HttpPut("{username}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update(string username, User user)
    {
        if (username != user.Username)
            return BadRequest(); // 400

        var existingUser = _userService.Get(user.Username);

        //if (existingUser is null)
            //return NotFound(); // 404

        _userService.Update(user);

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
        var user = _userService.Get(username);

        if (user is null)
            return NotFound(); // 404

        _userService.Delete(username);

        return NoContent(); // 204
    }
}