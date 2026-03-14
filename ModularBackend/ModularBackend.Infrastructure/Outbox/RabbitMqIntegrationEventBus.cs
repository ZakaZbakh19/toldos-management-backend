using ModularBackend.Application.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Outbox
{
    public sealed class RabbitMqIntegrationEventBus : IIntegrationEventBus
    {
        public Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        {
            // RabbitMQ aquí
            return Task.CompletedTask;
        }
    }
}
