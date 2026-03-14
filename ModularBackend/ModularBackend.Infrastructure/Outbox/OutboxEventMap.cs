using ModularBackend.Application.Abstractions.Events;
using ModularBackend.Application.IntegrationEvents;
using ModularBackend.Application.Products.Events;
using ModularBackend.Domain.Abstractions;
using ModularBackend.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ModularBackend.Infrastructure.Outbox
{
    public static class OutboxEventMap
    {
        public static IIntegrationEvent Map(OutboxMessage message)
        {
            if (message.Type == typeof(ProductCreatedIntegrationEvent).FullName)
            {
                var evt = JsonSerializer.Deserialize<ProductCreatedIntegrationEvent>(message.Payload)
                          ?? throw new InvalidOperationException("Invalid payload");

                return evt;
            }

            return null;
        }       
    }
}
