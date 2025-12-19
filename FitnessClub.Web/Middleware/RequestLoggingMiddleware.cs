using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FitnessClub.Web.Middleware
{
    public class RequestLoggingMiddleware  // Middleware voor gedetailleerd request logging
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();  // Start stopwatch voor timing
            var request = context.Request;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";  // Haal IP adres op

            // Log request start
            _logger.LogInformation($"Request started: {request.Method} {request.Path} from {ipAddress}");

            try
            {
                await _next(context);  // Verwerk request

                stopwatch.Stop();

                // Log completion met statuscode
                var statusCode = context.Response.StatusCode;
                var logLevel = statusCode >= 500 ? LogLevel.Error :  // Server error
                              statusCode >= 400 ? LogLevel.Warning : // Client error
                              LogLevel.Information;                  // Succes

                _logger.Log(logLevel,
                    $"Request completed: {request.Method} {request.Path} " +
                    $"=> {statusCode} in {stopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                // Log error met details
                _logger.LogError(ex,
                    $"Request failed: {request.Method} {request.Path} " +
                    $"=> Error: {ex.Message} in {stopwatch.ElapsedMilliseconds}ms");

                throw;  // Re-throw voor error handling middleware
            }
        }
    }

    // Extension method voor eenvoudige middleware registratie
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}