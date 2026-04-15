using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Mediator
{
    public interface ICommandRequest<TResponse> : IRequest<TResponse> { }
}
