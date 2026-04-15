using ModularBackend.Application.Abstractions.Events.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Services.Messaging
{
    public interface IMessagingBus
    {
        Task PublishAsync<TIntegrationEvent>(
            TIntegrationEvent integrationEvent,
            CancellationToken cancellationToken = default)
            where TIntegrationEvent : IIntegrationEvent;
    }
}
