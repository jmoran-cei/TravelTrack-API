using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using http = System.Web.Http;
using System.Net;
using Microsoft.Identity.Web.Resource;
using TravelTrack_API.Versions.v1.Models;
using TravelTrack_API.Versions.v1.Services;

namespace TravelTrack_API.Versions.v1.Controllers
{
    /// <summary>
    /// Handles incoming trip related Http Requests
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ControllerName("TripsV1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [EnableCors()]
    //[Authorize]
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
        /// Returns all trips, synchronous
        /// </summary>
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


        /// <summary>
        /// Returns a trip when given a existing trip Id, synchronous
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredScope("Trips.Read")]
        public ActionResult<TripDto> Get([FromRoute] long id)
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


        /// <summary>
        /// Creates a new trip, synchronous
        /// </summary>
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


        /// <summary>
        /// Updates a trip, synchronous
        /// </summary>
        [HttpPut("{id}")]
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


        /// <summary>
        /// Deletes a trip, synchronous
        /// </summary>
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



        /// <summary>
        /// Adds a photo to a trip and uploads the file to Azure blob storage, synchronous
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
        public IActionResult AddPhoto([FromForm] PhotoDto photo, [FromRoute] long id)
        {
            try
            {
                if (Request.Form.Files.Count() > 0)
                {
                    IFormFile file = Request.Form.Files[0];
                    var updatedTrip = _tripService.AddPhotoToTrip(photo, file, id);
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
        /// Removes photos from a trip and deletes the file from Azure blob storage, synchronous
        /// </summary>
        [HttpPut("removephotos/{id}")]
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


    }
}