using FluentValidation;
using ModularBackend.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Messaging.Mediator
{
    public sealed class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task<TResponse> Send<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new MediatorException("Request cannot be null.");

            var validatorType = typeof(IValidator<>)
                .MakeGenericType(request.GetType());

            var validator = _serviceProvider.GetService(validatorType);
            if (validator is not null)
            {
                var validationResult = await ((dynamic)validator).ValidateAsync((dynamic)request, cancellationToken);
                if (!validationResult.IsValid)
                    throw new ValidationException(validationResult.Errors);
            }

            var handlerType = typeof(IRequestHandler<,>)
                .MakeGenericType(request.GetType(), typeof(TResponse));

            var handler = _serviceProvider.GetService(handlerType);

            if (handler is null)
                throw new MediatorException(
                    $"No handler registered for request '{request.GetType().Name}' -> response '{typeof(TResponse).Name}'.");

            // Importante: esto invoca Handle sin reflection MethodInfo.Invoke
            // y por tanto no te envuelve la excepción real en TargetInvocationException.
            return await ((dynamic)handler).Handle((dynamic)request, cancellationToken);
        }
    }
}
