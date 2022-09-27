using TravelTrack_API.Services;
using TravelTrack_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Trips.Controllers;

[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    public TripsController() { }

    // GET all action
    [HttpGet]
    [ProducesResponseType(typeof(Trip[]), StatusCodes.Status200OK)]
    public ActionResult<List<Trip>> GetAll() =>
        Ok(TripService.GetAll()); // 200

    // GET by Id action
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Trip), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Trip> Get(int id)
    {
        var trip = TripService.Get(id);

        if (trip is null)
            return NotFound(); // 404

        return Ok(trip); // 200
    }

    // POST action
    [HttpPost]
    [ProducesResponseType(typeof(Trip), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create(Trip trip)
    {
        if (trip is null)
            return BadRequest(); // 400

        TripService.Add(trip);

        return CreatedAtAction(nameof(Create), "Trips", new { id = trip.Id }, trip); // 201
    }

    // PUT action
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, Trip trip)
    {
        if (id != trip.Id)
            return BadRequest(); // 400

        var existingTrip = TripService.Get(trip.Id);

        if (existingTrip is null)
            return NotFound(); // 404

        TripService.Update(trip);

        return Ok(trip); // 200
    }

    // DELETE action
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var trip = TripService.Get(id);

        if (trip is null)
            return NotFound(); // 404

        TripService.Delete(id);

        return NoContent(); // 204
    }
}