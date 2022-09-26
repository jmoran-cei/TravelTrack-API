using TravelTrack_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Users.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    public UsersController() { }

    // GET all action
    [HttpGet]
    public ActionResult<List<TravelTrack_API.Models.User>> GetAll() =>
        UserService.GetAll();

    // GET by Username action
    [HttpGet("{username}")]
    public ActionResult<TravelTrack_API.Models.User> Get(string username)
    {
        var user = UserService.Get(username);

        if (user == null)
            return NotFound();

        return user;
    }

    // POST action
    [HttpPost]
    public IActionResult Create(TravelTrack_API.Models.User user)
    {
        if (user is null) // is this fully redundant? or good practice
            return BadRequest();

        UserService.Add(user);

        return new CreatedAtActionResult(nameof(Create), "Users", new { username = user.Username }, user);
    }

    // PUT action
    [HttpPut("{username}")]
    public IActionResult Update(string username, TravelTrack_API.Models.User user)
    {
        if (username != user.Username)
            return BadRequest();

        var existingUser = UserService.Get(user.Username);

        if (existingUser is null)
            return NotFound();

        UserService.Update(user);

        return NoContent();
    }

    // DELETE action
    [HttpDelete("{username}")]
    public IActionResult Delete(string username)
    {
        var user = UserService.Get(username);

        if (user is null)
            return NotFound();

        UserService.Delete(username);

        return NoContent();
    }
}