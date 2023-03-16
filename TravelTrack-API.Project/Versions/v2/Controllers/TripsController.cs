using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using http = System.Web.Http;
using System.Net;
using Microsoft.Identity.Web.Resource;
using TravelTrack_API.Versions.v2.Models;
using TravelTrack_API.Versions.v2.Services;

namespace TravelTrack_API.Versions.v2.Controllers
{
    /// <summary>
    /// Handles incoming trip related Http Requests
    /// This version implements all of version 1.0 but asynchronously
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ControllerName("TripsV2")]
    [Route("api/v{version:apiVersion}/[controller]/")]
    [EnableCors()]
    [Authorize]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;
        private readonly ILogger<TripsController> _logger;

        public TripsController(ITripService tripService, ILogger<TripsController> logger)
        {
            _tripService = tripService;
            _logger = logger;
        }


        /// <summary>
        /// Returns all trips
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<TripDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredScope("Trips.Read")]
        public async Task<ActionResult<List<TripDto>>> GetAllAsync(ApiVersion version)
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
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredScope("Trips.Read")]
        public async Task<ActionResult<TripDto>> GetAsync([FromRoute] long id)
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
                return new CreatedAtActionResult(nameof(CreateAsync), "Trips", new { id = trip.Id }, trip); // 201
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
        [HttpPut("addphoto/{id}")]
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
        [HttpPut("removephotos/{id}")]
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
}
