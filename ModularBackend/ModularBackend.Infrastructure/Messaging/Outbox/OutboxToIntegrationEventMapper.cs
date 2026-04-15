using ModularBackend.Application.Abstractions.Events.IntegrationEvents;
using ModularBackend.Application.Features.Products.CreateProduct;
using System.Text.Json;

namespace ModularBackend.Infrastructure.Messaging.Outbox
{
    public static class OutboxToIntegrationEventMapper
    {
        private static readonly Dictionary<string, Func<string, IIntegrationEvent>> _eventTypeMap = new()
        {
            { IntegrationEventNames.ProductCreatedV1, Create<ProductCreatedIntegrationEvent>() }
        };

        private static Func<string, IIntegrationEvent> Create<T>()
            where T : IIntegrationEvent
        {
            return payload => JsonSerializer.Deserialize<T>(payload)!;
        }

        public static IIntegrationEvent? Map(OutboxMessage message)
        {
            if (_eventTypeMap.TryGetValue(message.Type, out var factory))
            {
                return factory(message.Payload);
            }

            return null;
        }
    }
}
