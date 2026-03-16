using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.IntegrationEvents
{
    public interface IProcessedIntegrationEventStore
    {
        Task<bool> HasBeenProcessedAsync(Guid eventId, CancellationToken cancellationToken);
        Task MarkAsProcessedAsync(Guid eventId, string eventType, CancellationToken cancellationToken);
    }
}
