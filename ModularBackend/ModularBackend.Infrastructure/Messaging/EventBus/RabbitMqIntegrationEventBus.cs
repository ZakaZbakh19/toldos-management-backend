using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularBackend.Application.Abstractions.Events.IntegrationEvents;
using ModularBackend.Application.Abstractions.Services.Messaging;
using ModularBackend.Application.Features.Products.CreateProduct;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ModularBackend.Infrastructure.Messaging.EventBus
{

    public sealed class RabbitMqIntegrationEventBus : IMessagingBus
    {
        private readonly IConnection _connection;
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqIntegrationEventBus> _logger;

        public RabbitMqIntegrationEventBus(
            IOptions<RabbitMqOptions> options,
            ILogger<RabbitMqIntegrationEventBus> logger,
            IConnection connection)
        {
            _options = options.Value;
            _logger = logger;
            _connection = connection;
        }

        public async Task PublishAsync<TIntegrationEvent>(
           TIntegrationEvent integrationEvent,
           CancellationToken cancellationToken = default)
           where TIntegrationEvent : IIntegrationEvent
        {
            var routingKey = integrationEvent switch
            {
                ProductCreatedIntegrationEvent => IntegrationEventNames.ProductCreatedV1,
                _ => throw new InvalidOperationException(
                    $"Unknown integration event type: {integrationEvent.GetType().Name}")
            };

            var createChannelOptions = new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true,
                consumerDispatchConcurrency: 1);

            await using var channel = await _connection.CreateChannelAsync(createChannelOptions, cancellationToken);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(integrationEvent));

            var props = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                MessageId = integrationEvent.EventId.ToString()
            };

            await channel.BasicPublishAsync(
                exchange: _options.ExchangeName,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: props,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Published integration event {EventType} with EventId {EventId}",
                routingKey,
                integrationEvent.EventId);
        }
    }
}
