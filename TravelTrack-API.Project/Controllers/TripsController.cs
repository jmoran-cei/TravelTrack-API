using TravelTrack_API.Services;
using TravelTrack_API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using http = System.Web.Http;
using System.Net;
using Microsoft.Identity.Web.Resource;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authorization;

namespace Trips.Controllers;

/// <summary>
/// Handles incoming trip related Http Requests
/// </summary>
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
[Route("api/[controller]")]
[EnableCors()]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;
    private readonly ILogger<TripsController> _logger;

    /// <summary>
    /// TripsController's constructor
    /// </summary>
    public TripsController(ITripService tripService, ILogger<TripsController> logger)
    {
        _tripService = tripService;
        _logger = logger;
    }

    /// <summary>
    /// Returns all trips
    /// </summary>
    [ApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(List<TripDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [RequiredScope("Trips.Read")]
    public ActionResult<List<TripDto>> GetAll()
    {
        try
        {
            return new OkObjectResult(_tripService.GetAll()); // 200
        }
        catch (Exception e)
        {
            // log to Application Insights
            _logger.LogError(e, e.Message);

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [ApiVersion("2.0")]
    [HttpGet]
    [ProducesResponseType(typeof(List<TripDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [RequiredScope("Trips.Read")]
    public async Task<ActionResult<List<TripDto>>> GetAllAsync()
    {
        try
        {
            return new OkObjectResult(await _tripService.GetAllAsync()); // 200
        }
        catch (Exception e)
        {
            // log to Application Insights
            _logger.LogError(e, e.Message);

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }



    /// <summary>
    /// Returns a trip when given a existing trip Id
    /// </summary>
    [ApiVersion("1.0")]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [RequiredScope("Trips.Read")]
    public ActionResult<TripDto> Get(long id)
    {
        try
        {
            return new OkObjectResult(_tripService.Get(id)); // 200
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

    [ApiVersion("2.0")]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [RequiredScope("Trips.Read")]
    public async Task<ActionResult<TripDto>> GetAsync(long id)
    {
        try
        {
            return new OkObjectResult(await _tripService.GetAsync(id)); // 200
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
    /// Creates a new trip
    /// </summary>
    [ApiVersion("1.0")]
    [HttpPost]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [RequiredScope("Trips.Write")]
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
            if (e.Response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new BadRequestObjectResult(e.Response); // 400
            }
            // log to Application Insights
            _logger.LogError(e, e.Response.ToString());

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        catch (Exception e)
        {
            // log to Application Insights
            _logger.LogError(e, e.Message);

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [ApiVersion("2.0")]
    [HttpPost]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [RequiredScope("Trips.Write")]
    public async Task<IActionResult> CreateAsync(TripDto trip)
    {
        try
        {
            var addedTrip = await _tripService.AddAsync(trip);
            return new CreatedAtActionResult(nameof(Create), "Trips", new { id = trip.Id }, trip); // 201
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
            _logger.LogError(e, e.Response.ToString());

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
    /// Updates a trip
    /// </summary>
    [HttpPut("{id}")]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [RequiredScope("Trips.Write")]
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
            if (e.Response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new BadRequestObjectResult(e.Response); // 400
            }
            // log to Application Insights
            _logger.LogError(e, e.Response.ToString());

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        catch (Exception e)
        {
            // log to Application Insights
            _logger.LogError(e, e.Message);

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("{id}")]
    [ApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [RequiredScope("Trips.Write")]
    public async Task<IActionResult> UpdateAsync(long id, TripDto trip)
    {
        try
        {
            return new OkObjectResult(await _tripService.UpdateAsync(id, trip));
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
            _logger.LogError(e, e.Response.ToString());

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
    /// Deletes a trip
    /// </summary>
    [ApiVersion("1.0")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [RequiredScope("Trips.Write")]
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
            // log to Application Insights
            _logger.LogError(e, e.Response.ToString());

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        catch (Exception e)
        {
            // log to Application Insights
            _logger.LogError(e, e.Message);


            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [ApiVersion("2.0")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [RequiredScope("Trips.Write")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        try
        {
            await _tripService.DeleteAsync(id);
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
            _logger.LogError(e, e.Response.ToString());

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
    /// Adds a photo to a trip and uploads the file to Azure blob storage
    /// </summary>
    [ApiVersion("1.0")]
    [HttpPut("{id}/addphoto")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [RequiredScope("Trips.Write")]
    public IActionResult AddPhoto([FromForm] PhotoDto photo, [FromRoute]long id)
    {
        try
        {
            if (Request.Form.Files.Count() > 0)
            {
                IFormFile file = Request.Form.Files[0];
                var updatedTrip =_tripService.AddPhotoToTrip(photo, file, id);
                return new OkObjectResult(updatedTrip);
            }
            else
            {
                throw new http.HttpResponseException( //400
                    _tripService.ResponseMessage(
                        HttpStatusCode.BadRequest,
                        "Null File: Make sure to provide a file as form data",
                        "Bad Request: Null File"
                    )
                );
            }
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
            // log to Application Insights
            _logger.LogError(e, e.Response.ToString());

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        catch (Exception e)
        {
            // log to Application Insights
            _logger.LogError(e, e.Message);

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [ApiVersion("2.0")]
    [HttpPut("{id}/addphoto")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [RequiredScope("Trips.Write")]
    public async Task<IActionResult> AddPhotoAsync([FromForm] PhotoDto photo, [FromRoute] long id)
    {
        try
        {
            if (Request.Form.Files.Count() > 0)
            {
                IFormFile file = Request.Form.Files[0];
                var updatedTrip = await _tripService.AddPhotoToTripAsync(photo, file, id);
                return new OkObjectResult(updatedTrip);
            }
            else
            {
                throw new http.HttpResponseException( //400
                    _tripService.ResponseMessage(
                        HttpStatusCode.BadRequest,
                        "Null File: Make sure to provide a file as form data",
                        "Bad Request: Null File"
                    )
                );
            }
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
            // log to Application Insights
            _logger.LogError(e, e.Response.ToString());

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
    /// Removes photos from a trip and deletes the file from Azure blob storage
    /// </summary>
    [ApiVersion("1.0")]
    [HttpPut("{id}/removephotos")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [RequiredScope("Trips.Write")]
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
            // log to Application Insights
            _logger.LogError(e, e.Response.ToString());

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        catch (Exception e)
        {
            // log to Application Insights
            _logger.LogError(e, e.Message);

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [ApiVersion("2.0")]
    [HttpPut("{id}/removephotos")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [RequiredScope("Trips.Write")]
    public async Task<IActionResult> RemovePhotosAsync(List<PhotoDto> photos, [FromRoute] long id)
    {
        try
        {
            var updatedTrip = await _tripService.RemovePhotosFromTripAsync(photos, id);
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
            // log to Application Insights
            _logger.LogError(e, e.Response.ToString());

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