using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FitnessClub.Web.Middleware
{
    public class RequestLoggingMiddleware
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
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            // Log request start
            _logger.LogInformation($"Request started: {request.Method} {request.Path} from {ipAddress}");

            try
            {
                // Request
                await _next(context);

                stopwatch.Stop();

                // Log completion
                var statusCode = context.Response.StatusCode;
                var logLevel = statusCode >= 500 ? LogLevel.Error :
                              statusCode >= 400 ? LogLevel.Warning :
                              LogLevel.Information;

                _logger.Log(logLevel,
                    $"Request completed: {request.Method} {request.Path} " +
                    $"=> {statusCode} in {stopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                // Log error
                _logger.LogError(ex,
                    $"Request failed: {request.Method} {request.Path} " +
                    $"=> Error: {ex.Message} in {stopwatch.ElapsedMilliseconds}ms");

                throw; // Re-throw voor middleware 
            }
        }
    }

    //  method registratie
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}