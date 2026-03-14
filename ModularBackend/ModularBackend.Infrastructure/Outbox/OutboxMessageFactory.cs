using ModularBackend.Application.IntegrationEvents;
using ModularBackend.Domain.Abstractions;
using ModularBackend.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ModularBackend.Infrastructure.Outbox
{
    public static class OutboxMessageFactory
    {
        public static IReadOnlyCollection<OutboxMessage> Create(IEnumerable<IDomainEvent> domainEvents)
        {
            var messages = new List<OutboxMessage>();

            foreach (var domainEvent in domainEvents)
            {
                switch (domainEvent)
                {
                    case ProductCreatedDomainEvent e:
                        var integrationEvent = new ProductCreatedIntegrationEvent(
                            name: e.Name,
                            price: e.Price
                        );

                        messages.Add(new OutboxMessage
                        {
                            Id = Guid.NewGuid(),
                            Type = typeof(ProductCreatedIntegrationEvent).FullName!,
                            Payload = JsonSerializer.Serialize(integrationEvent),
                            OccurredOnUtc = DateTime.UtcNow
                        });
                        break;
                }
            }

            return messages;
        }
    }
}
