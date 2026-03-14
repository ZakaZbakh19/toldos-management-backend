using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.IntegrationEvents
{
    public interface IIntegrationEventBus
    {
        Task PublishAsync<TIntegrationEvent>(
            TIntegrationEvent integrationEvent,
            CancellationToken cancellationToken = default);
    }
}
