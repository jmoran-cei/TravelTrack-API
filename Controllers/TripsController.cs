using TravelTrack_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Trips.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    public TripsController() { }

    // GET all action
    [HttpGet]
    public ActionResult<List<TravelTrack_API.Models.Trip>> GetAll() =>
        TripService.GetAll();

    // GET by Id action
    [HttpGet("{id}")]
    public ActionResult<TravelTrack_API.Models.Trip> Get(int id)
    {
        var trip = TripService.Get(id);

        if (trip == null)
            return NotFound();

        return trip;
    }

    // POST action
    [HttpPost]
    public IActionResult Create(TravelTrack_API.Models.Trip trip)
    {
        if (trip is null)
            return BadRequest();

        TripService.Add(trip);

        return new CreatedAtActionResult(nameof(Create), "Trips", new { id = trip.Id }, trip);
    }

    // PUT action
    [HttpPut("{id}")]
    public IActionResult Update(int id, TravelTrack_API.Models.Trip trip)
    {
        if (id != trip.Id)
            return BadRequest();

        var existingTrip = TripService.Get(trip.Id);

        if (existingTrip is null)
            return NotFound();

        TripService.Update(trip);

        return NoContent();
    }

    // DELETE action
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var trip = TripService.Get(id);

        if (trip is null)
            return NotFound();

        TripService.Delete(id);

        return NoContent();
    }
}