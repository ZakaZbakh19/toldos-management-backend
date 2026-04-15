using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Events.IntegrationEvents
{
    public interface IIntegrationEventDispatcher
    {
        Task DispatchAsync(
            string messageId,
            string eventType,
            string payload,
            string consumerName,
            CancellationToken cancellationToken);
    }
}
