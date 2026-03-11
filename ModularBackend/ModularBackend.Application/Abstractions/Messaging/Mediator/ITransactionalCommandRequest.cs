using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Messaging.Mediator
{
    public interface ITransactionalCommandRequest<TResponse> : ICommandRequest<TResponse> { }
}
