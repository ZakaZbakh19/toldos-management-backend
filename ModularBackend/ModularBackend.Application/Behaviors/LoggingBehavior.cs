using Microsoft.Extensions.Logging;
using ModularBackend.Application.Mediator;
using System.Diagnostics;

namespace ModularBackend.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var timer = Stopwatch.StartNew(); // Inicializamos el cronómetro

        _logger.LogInformation("Handling {Request}", requestName);

        try
        {
            var response = await next();

            timer.Stop(); // Detenemos al finalizar el 'next'
            var elapsed = timer.ElapsedMilliseconds;

            _logger.LogInformation("Handled {Request} in {Elapsed}ms", requestName, elapsed);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling {Request}", requestName);
            throw;
        }
    }
}
