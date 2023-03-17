using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using http = System.Web.Http;
using System.Net;
using Microsoft.Identity.Web.Resource;
using TravelTrack_API.Versions.v1.DtoModels;
using TripDto = TravelTrack_API.Versions.v2.DtoModels.TripDto;
using TravelTrack_API.SharedServices;

namespace TravelTrack_API.Versions.v2.Controllers
{
    /// <summary>
    /// Handles incoming trip related Http Requests
    /// This version implements the Azure Directory B2C connection to the API and appropriate changes needed map TripDto.Members with Trip.B2CMembers entity
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ControllerName("TripsV2")]
    [Route("api/[controller]")] // newest version is default
    [Route("api/[controller]/v{version:apiVersion}")]
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
        /// Returns a trip when given a existing userId, the trip member data is from respective AD B2C users
        /// </summary>
        [HttpGet]
        [Route("user/{userId}")]
        [ProducesResponseType(typeof(List<TripDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TripDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredScope("Trips.Read")]
        public async Task<ActionResult<List<TripDto>>> GetTripsByUserIdAsync([FromRoute] string userId, ApiVersion version)
        {
            try
            {
                //var userId = "7241d571-59ec-40f7-8730-84de1ff982d6";
                return new OkObjectResult(await _tripService.GetTripsByUserIdAsync(userId)); // 200
            }
            catch (http.HttpResponseException e)
            {
                if (e.Response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return new BadRequestObjectResult(e.Response); // 400
                }
                if (e.Response.StatusCode == HttpStatusCode.NoContent)
                {
                    return new NoContentResult(); // 204
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
        /// Returns a trip when given a existing trip Id, the trip member data is from respective AD B2C users
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredScope("Trips.Read")]
        public async Task<ActionResult<TripDto>> GetTripAsync([FromRoute] long id)
        {
            try
            {
                return new OkObjectResult(await _tripService.GetTripB2CAsync(id)); // 200
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
        /// Creates a new trip, the trip member data is from respective AD B2C users
        /// </summary>
        [HttpPost,]
        [ProducesResponseType(typeof(TripDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [RequiredScope("Trips.Write")]
        public async Task<IActionResult> CreateAsync(TripDto trip)
        {
            try
            {
                var addedTrip = await _tripService.AddAsync(trip);
                return new CreatedAtActionResult("Create", "TripsV2", new { id = trip.Id }, trip); // 201
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
        /// Updates a trip, the trip member data is from respective AD B2C users
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
        /// Removes a trip when provided an existing trip id
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
        /// Adds a photo to a trip and uploads the file to Azure blob storage, the trip member data is from respective AD B2C users
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
                    var updatedTrip = await _tripService.AddPhotoToB2CTripAsync(photo, file, id);
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
        /// Removes photos from a trip and deletes the file from Azure blob storage, the trip member data is from respective AD B2C users
        /// </summary>
        [HttpPut("removephotos/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredScope("Trips.Write")]
        public async Task<IActionResult> RemovePhotosFromB2CTripAsync(List<PhotoDto> photos, [FromRoute] long id)
        {
            try
            {
                var updatedTrip = await _tripService.RemovePhotosFromB2CTripAsync(photos, id);
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
