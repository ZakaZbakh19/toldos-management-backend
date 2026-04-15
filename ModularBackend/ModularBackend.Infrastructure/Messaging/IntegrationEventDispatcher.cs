using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ModularBackend.Application.Abstractions.Events.IntegrationEvents;
using ModularBackend.Application.Abstractions.Services.Messaging;
using ModularBackend.Application.Features.Products.CreateProduct;
using System.Text.Json;

namespace ModularBackend.Infrastructure.Messaging
{
    public sealed class IntegrationEventDispatcher : IIntegrationEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IProcessedIntegrationEventStore _processedStore;

        // El diccionario mapea el nombre del string al Tipo de la clase C#
        private static readonly Dictionary<string, Type> _eventTypeMapping = new()
        {
            { "ProductCreatedV1", typeof(ProductCreatedIntegrationEvent) },
        };

        public IntegrationEventDispatcher(
            IServiceProvider serviceProvider,
            IProcessedIntegrationEventStore processedStore)
        {
            _serviceProvider = serviceProvider;
            _processedStore = processedStore;
        }

        public async Task DispatchAsync(
            string messageIdRaw,
            string eventType,
            string payload,
            string consumerName,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(messageIdRaw, out var eventId)) return;

            if (await _processedStore.HasBeenProcessedAsync(eventId, consumerName, cancellationToken))
                return;

            try
            {
                if (_eventTypeMapping.TryGetValue(eventType, out var type))
                {
                    var evt = JsonSerializer.Deserialize(payload, type);

                    var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(type);
                    var handler = _serviceProvider.GetRequiredService(handlerType);

                    var method = handlerType.GetMethod("HandleAsync");
                    if (method != null)
                    {
                        await (Task)method.Invoke(handler, new[] { evt, cancellationToken })!;
                    }
                }

                await _processedStore.MarkAsProcessedAsync(eventId, consumerName, eventType, cancellationToken);
            }
            catch (DbUpdateException) // EF Core lanza esto cuando falla la restricción de SQL
            {
            }
        }
    }
}
