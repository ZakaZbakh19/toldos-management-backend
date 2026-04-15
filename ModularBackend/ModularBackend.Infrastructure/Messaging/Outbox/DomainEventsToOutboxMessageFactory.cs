using ModularBackend.Application.Abstractions.Events.IntegrationEvents;
using ModularBackend.Application.Features.Products.CreateProduct;
using ModularBackend.Domain.Abstractions;
using ModularBackend.Domain.DomainEvents;
using ModularBackend.Domain.Events;
using System.Text.Json;

namespace ModularBackend.Infrastructure.Messaging.Outbox
{
    public static class DomainEventsToOutboxMessageFactory
    {
        private static readonly Dictionary<string, Func<IDomainEvent, OutboxMessage>> _map = new()
        {
            {
                DomainEventNames.ProductCreatedDomainEvent,
                Create<ProductCreatedDomainEvent>(e =>
                {
                    var integrationEvent = new ProductCreatedIntegrationEvent(
                        EventId: Guid.NewGuid(),
                        OccurredOnUtc: DateTime.UtcNow,
                        ProductId: e.ProductId,
                        Name: e.Name,
                        Price: e.Price);

                    return new OutboxMessage
                    {
                        Id = Guid.NewGuid(),
                        Type = IntegrationEventNames.ProductCreatedV1,
                        Payload = JsonSerializer.Serialize(integrationEvent),
                        OccurredOnUtc = DateTime.UtcNow,
                        Attempts = 0
                    };
                })
            }
        };

        private static Func<IDomainEvent, OutboxMessage> Create<TDomainEvent>(
            Func<TDomainEvent, OutboxMessage> map)
            where TDomainEvent : IDomainEvent
        {
            return domainEvent => map((TDomainEvent)domainEvent);
        }

        public static IReadOnlyCollection<OutboxMessage> Map(IEnumerable<IDomainEvent> domainEvents)
        {
            var messages = new List<OutboxMessage>();

            foreach (var domainEvent in domainEvents)
            {
                if (_map.TryGetValue(domainEvent.Type, out var factory))
                {
                    var message = factory(domainEvent);
                    messages.Add(message);
                }
            }

            return messages;
        }
    }
}
