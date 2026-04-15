using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Services.Messaging
{
    public interface IProcessedIntegrationEventStore
    {
        Task<bool> HasBeenProcessedAsync(
            Guid eventId,
            string consumerName,
            CancellationToken cancellationToken);

        Task MarkAsProcessedAsync(
            Guid eventId,
            string consumerName,
            string eventType,
            CancellationToken cancellationToken);
    }
}
