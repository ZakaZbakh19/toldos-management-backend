using System.Diagnostics;

namespace ModularBackend.Api.Middlewares
{
    public sealed class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(
            RequestDelegate next,
            ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation(
                "Incoming HTTP request {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                traceId);

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var elapsedMs = stopwatch.ElapsedMilliseconds;
                var logLevel = elapsedMs > 1000
                    ? LogLevel.Warning
                    : LogLevel.Information;

                _logger.Log(logLevel,
                    "Completed HTTP request {Method} {Path} with status code {StatusCode} in {ElapsedMs} ms. TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    elapsedMs,
                    traceId);
            }
        }
    }
}
