using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace FitnessClub.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;

            // Log request informatie
            _logger.LogInformation("🔹 INKOMENDE REQUEST");
            _logger.LogInformation($"   Method: {request.Method}");
            _logger.LogInformation($"   Path: {request.Path}");
            _logger.LogInformation($"   Query: {request.QueryString}");
            _logger.LogInformation($"   Client IP: {context.Connection.RemoteIpAddress}");

            // Log headers (optioneel)
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                _logger.LogInformation($"   Auth: {authHeader.Substring(0, Math.Min(20, authHeader.Length))}...");
            }

            // Log request body voor POST/PUT (behalve voor grote uploads)
            if ((request.Method == "POST" || request.Method == "PUT") &&
                request.ContentLength > 0 && request.ContentLength < 4096)
            {
                request.EnableBuffering();

                using var reader = new StreamReader(
                    request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;

                if (!string.IsNullOrWhiteSpace(body))
                {
                    _logger.LogInformation($"   Body: {body}");
                }
            }

            // Vang de response op
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
                stopwatch.Stop();

                // Log response
                _logger.LogInformation($"✅ RESPONSE ({stopwatch.ElapsedMilliseconds}ms)");
                _logger.LogInformation($"   Status: {context.Response.StatusCode}");

                // Log response body (voor errors of kleine responses)
                if (context.Response.StatusCode >= 400 ||
                   (context.Response.ContentLength.HasValue &&
                    context.Response.ContentLength < 1024))
                {
                    responseBody.Seek(0, SeekOrigin.Begin);
                    var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                    responseBody.Seek(0, SeekOrigin.Begin);

                    if (!string.IsNullOrWhiteSpace(responseText))
                    {
                        _logger.LogInformation($"   Response: {responseText}");
                    }
                }

                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"❌ REQUEST MISLUKT na {stopwatch.ElapsedMilliseconds}ms");

                // Toon vriendelijke error aan gebruiker
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    success = false,
                    message = "Er is een fout opgetreden in de server",
                    errorId = Guid.NewGuid().ToString()
                };

                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(jsonResponse);

                _logger.LogInformation($"   Error Response: {jsonResponse}");
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}