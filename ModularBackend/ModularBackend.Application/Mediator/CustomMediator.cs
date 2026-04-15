using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ModularBackend.Application.Exceptions;

namespace ModularBackend.Application.Mediator;

public sealed class CustomMediator : IMediator
{
    private static readonly MethodInfo SendInternalMethod =
        typeof(CustomMediator).GetMethod(nameof(SendInternal), BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("Mediator internal send method was not found.");

    private readonly IServiceProvider _serviceProvider;

    public CustomMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var closedMethod = SendInternalMethod.MakeGenericMethod(request.GetType(), typeof(TResponse));
        var task = (Task<TResponse>?)closedMethod.Invoke(this, new object[] { request, cancellationToken });

        return task ?? throw new MediatorException(
            $"Mediator could not dispatch request '{request.GetType().Name}'.");
    }

    private Task<TResponse> SendInternal<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest<TResponse>
    {
        var handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        var behaviors = _serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>().Reverse().ToArray();

        RequestHandlerDelegate<TResponse> next = () => handler.Handle(request, cancellationToken);

        foreach (var behavior in behaviors)
        {
            var currentNext = next;
            next = () => behavior.Handle(request, currentNext, cancellationToken);
        }

        return next();
    }
}

