using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.IntegrationEvents
{
    public interface IIntegrationEvent
    {
        Guid EventId { get; }
        DateTime OccurredOnUtc { get; }
    }
}
