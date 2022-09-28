using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TravelTrack_API.Controllers;

///<summary>
/// handles incoming errors (when not in Development)
///</summary>
[ApiController]
public class ErrorController : ControllerBase
{

    ///<summary>
    /// Returns error
    ///</summary>
    [HttpGet("api/error")]
    public IActionResult Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var stackTrace = context?.Error.StackTrace;
        var errorMessage = context?.Error.Message;

        // log error somewhere in the future

        return Problem();
    }
}