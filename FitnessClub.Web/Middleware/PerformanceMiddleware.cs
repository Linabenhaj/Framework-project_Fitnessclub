namespace FitnessClub.Web.Middleware
{
    public class PerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMiddleware> _logger;

        public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;

            // Add custom header to track request
            context.Response.OnStarting(() =>
            {
                var duration = DateTime.UtcNow - startTime;
                context.Response.Headers.Append("X-Request-Duration", $"{duration.TotalMilliseconds}ms");

                // Warn if request takes too long
                if (duration.TotalSeconds > 3)
                {
                    _logger.LogWarning($"Slow request detected: {context.Request.Path} took {duration.TotalSeconds}s");
                }

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}