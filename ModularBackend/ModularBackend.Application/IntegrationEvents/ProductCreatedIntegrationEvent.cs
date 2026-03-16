using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.IntegrationEvents
{
    public sealed record ProductCreatedIntegrationEvent(
        Guid EventId,
        DateTime OccurredOnUtc,
        Guid ProductId,
        string Name,
        decimal Price) : IIntegrationEvent;
}
