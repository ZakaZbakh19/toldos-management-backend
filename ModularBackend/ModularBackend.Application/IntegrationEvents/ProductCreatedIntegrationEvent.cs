using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.IntegrationEvents
{
    public sealed record ProductCreatedIntegrationEvent(string name, decimal price) : IIntegrationEvent
    {
        public Guid EventId => Guid.NewGuid();
        public DateTime OccurredOnUtc => DateTime.UtcNow;
    }
}
