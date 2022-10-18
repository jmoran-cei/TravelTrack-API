using TravelTrack_API.Services;
using TravelTrack_API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace Trips.Controllers;

/// <summary>
/// Handles incoming trip related Http Requests
/// </summary>
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
[Route("api/[controller]")]
[EnableCors()]
[ApiVersion("1.0")]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;

    /// <summary>
    /// TripsController's constructor
    /// </summary>
    public TripsController(ITripService tripService)
    {
        _tripService = tripService;
    }

    /// <summary>
    /// Returns all trips
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TripDto[]), StatusCodes.Status200OK)]
    public ActionResult<List<TripDto>> GetAll() =>
        Ok(_tripService.GetAll()); // 200
        //Ok(TripServiceTEMP.GetAll()); // 200

    /// <summary>
    /// Returns a trip when given a existing trip Id
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<TripDto> Get(long id)
    {
        //var trip = _tripService.Get(id);
        var trip = TripServiceTEMP.Get(id);

        if (trip is null)
            return NotFound(); // 404

        return Ok(trip); // 200
    }

    /// <summary>
    /// Creates a new trip
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create(TripDto trip)
    {
        if (trip is null)
            return BadRequest(); // 400

        //_tripService.Add(trip);
        TripServiceTEMP.Add(trip);

        return CreatedAtAction(nameof(Create), "Trips", new { id = trip.Id }, trip); // 201
    }

    /// <summary>
    /// Updates a trip
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update(long id, TripDto trip)
    {
        if (id != trip.Id)
            return BadRequest(); // 400

        //var existingTrip = _tripService.Get(trip.Id);
        var existingTrip = TripServiceTEMP.Get(trip.Id);

        if (existingTrip is null)
            return NotFound(); // 404

        //_tripService.Update(trip);
        TripServiceTEMP.Update(trip);

        return Ok(trip); // 200
    }

    /// <summary>
    /// Deletes a trip
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(long id)
    {
        //var trip = _tripService.Get(id)!;
        var trip = TripServiceTEMP.Get(id)!;

        if (trip is null)
            return NotFound(); // 404

        //_tripService.Delete(id);
        TripServiceTEMP.Delete(id);

        return NoContent(); // 204
    }
}