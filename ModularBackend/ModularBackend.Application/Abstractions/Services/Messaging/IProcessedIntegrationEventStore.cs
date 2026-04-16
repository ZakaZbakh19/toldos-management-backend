using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Services.Messaging
{
    public interface IProcessedIntegrationEventStore
    {
        Task<bool> TryStartProcessingAsync(
            Guid eventId,
            string consumerName,
            string eventType,
            CancellationToken cancellationToken);

        Task MarkAsProcessedAsync(
            Guid eventId,
            string consumerName,
            CancellationToken cancellationToken);

        Task MarkAsFailedAsync(
            Guid eventId,
            string consumerName,
            string error,
            CancellationToken cancellationToken);
    }
}
