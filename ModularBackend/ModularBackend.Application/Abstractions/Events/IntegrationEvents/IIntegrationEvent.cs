using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Events.IntegrationEvents
{
    public interface IIntegrationEvent
    {
        Guid EventId { get; }
        DateTime OccurredOnUtc { get; }
    }
}
