namespace FitnessClub.Web.Middleware
{
    public class PerformanceMiddleware  // Middleware voor het monitoren van request performance
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
            var startTime = DateTime.UtcNow;  // Start tijd meten

            // Voeg custom header toe voor request tracking
            context.Response.OnStarting(() =>
            {
                var duration = DateTime.UtcNow - startTime;  // Bereken duur
                context.Response.Headers.Append("X-Request-Duration", $"{duration.TotalMilliseconds}ms");  // Voeg duur toe aan header

                // Waarschuw als request te lang duurt (>3 seconden)
                if (duration.TotalSeconds > 3)
                {
                    _logger.LogWarning($"Slow request detected: {context.Request.Path} took {duration.TotalSeconds}s");
                }

                return Task.CompletedTask;
            });

            await _next(context);  // Ga naar volgende middleware
        }
    }
}