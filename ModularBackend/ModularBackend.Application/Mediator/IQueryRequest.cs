using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Mediator
{
    public interface IQueryRequest<TResponse> : IRequest<TResponse> { }
}
