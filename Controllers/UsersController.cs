using TravelTrack_API.Services;
using TravelTrack_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Users.Controllers;

/// <summary>
/// Handles incoming user related Http Requests
/// </summary>
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    /// <summary>
    /// UsersController's constructor
    /// </summary>
    public UsersController() { }

    /// <summary>
    /// Returns all users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(User[]), StatusCodes.Status200OK)]
    public ActionResult<List<User>> GetAll() =>
        Ok(UserService.GetAll()); // 200

    /// <summary>
    /// Returns a user when given an existing username
    /// </summary>
    [HttpGet("{username}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<User> Get(string username)
    {
        var user = UserService.Get(username);

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

        UserService.Add(user);

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

        var existingUser = UserService.Get(user.Username);

        if (existingUser is null)
            return NotFound(); // 404

        UserService.Update(user);

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
        var user = UserService.Get(username);

        if (user is null)
            return NotFound(); // 404

        UserService.Delete(username);

        return NoContent(); // 204
    }
}