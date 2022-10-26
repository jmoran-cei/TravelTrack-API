using System.Security.Cryptography;

namespace TravelTrack_API.Authorization
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;

        private const string APIKEYNAME = "X-Api-Key";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key for accessing TravelTrackAPI was not provided.");

                return;
            }

            //string apiKey = "test"; // uncomment for simplicity when testing using Swagger
            string apiKey = Secrets.ApiKey;

            // prevents "timing attacks" vulnerability if ApiKey is encrypted (instead of using apiKey.Equals(extractedApiKey))
            var equals = CryptographicOperations.FixedTimeEquals(
                System.Text.Encoding.UTF8.GetBytes(apiKey),
                System.Text.Encoding.UTF8.GetBytes(extractedApiKey));

            if (!equals)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client; unable to allow access to TravelTrackAPI.");

                return;
            }

            await _next(context);
        }
    }
}
