using TravelTrack_API.Services;
using TravelTrack_API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using http = System.Web.Http;
using System.Net;

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
    public ActionResult<List<TripDto>> GetAll()
    {
        return new OkObjectResult(_tripService.GetAll()); // 200
    }

    /// <summary>
    /// Returns a trip when given a existing trip Id
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<TripDto> Get(long id)
    {
        try
        {
            return new OkObjectResult(_tripService.Get(id)); // 200
        }
        catch (http.HttpResponseException e)
        {
            return new NotFoundObjectResult(e.Response); // 404
        }
    }

    /// <summary>
    /// Creates a new trip
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create(TripDto trip)
    {
        try
        {
            var addedTrip = _tripService.Add(trip);
            return new CreatedAtActionResult(nameof(Create), "Trips", new { id = trip.Id }, trip); // 201
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
    /// Updates a trip
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update(long id, TripDto trip)
    {
        try
        {
            return new OkObjectResult(_tripService.Update(id, trip));
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
    /// Deletes a trip
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(long id)
    {
        try
        {
            _tripService.Delete(id);
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
            return new ObjectResult(e.Response); // other (e.g. Azure blob throws error)
        }
    }

    /// <summary>
    /// Adds a photo to a trip and uploads the file to Azure blob storage
    /// </summary>
    [HttpPut("{id}/addphoto")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult AddPhoto([FromForm] PhotoDto photo, [FromRoute]long id)
    {
        try
        {
            IFormFile file = Request.Form.Files[0];
            var updatedTrip = _tripService.AddPhotoToTrip(photo, file, id);
            return new OkObjectResult(updatedTrip);
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
            if (e.Response.StatusCode == HttpStatusCode.Conflict)
            {
                return new ConflictObjectResult(e.Response); // 409
            }
            return new ObjectResult(e.Response); // other (e.g. Azure blob throws error)
        }
    }

    /// <summary>
    /// Removes photos from a trip and deletes the file from Azure blob storage
    /// </summary>
    [HttpPut("{id}/removephotos")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult RemovePhotos(List<PhotoDto> photos, [FromRoute] long id)
    {
        try
        {
            var updatedTrip = _tripService.RemovePhotosFromTrip(photos, id);
            return new OkObjectResult(updatedTrip);
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
            return new ObjectResult(e.Response); // other (e.g. Azure blob throws error)
        }
    }

}