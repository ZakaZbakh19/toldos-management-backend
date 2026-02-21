using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Messaging.Mediator
{
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    }
}
