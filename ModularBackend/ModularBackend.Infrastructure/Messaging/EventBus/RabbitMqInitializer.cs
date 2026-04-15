using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularBackend.Application.Abstractions.Events.IntegrationEvents;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Messaging.EventBus
{
    public sealed class RabbitMqInitializer
    {
        private readonly IConnection _connection;
        private readonly RabbitMqOptions _options;

        public RabbitMqInitializer(
            IOptions<RabbitMqOptions> options,
            IConnection connection)
        {
            _options = options.Value;
            _connection = connection;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            var createChannelOptions = new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true,
                consumerDispatchConcurrency: 1);

            await using var channel = await _connection.CreateChannelAsync(createChannelOptions, cancellationToken);

            const string retryExchange = "integration-events-retry";
            const string dlxExchange = "integration-events-dlx";

            await channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await channel.ExchangeDeclareAsync(
                exchange: retryExchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await channel.ExchangeDeclareAsync(
                exchange: dlxExchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: "catalog-products",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object?>
                {
                    { "x-dead-letter-exchange", retryExchange }
                },
                cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: "catalog-products-retry",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object?>
                {
                    { "x-message-ttl", 5000 },
                    { "x-dead-letter-exchange", _options.ExchangeName }
                },
                cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: "catalog-products-dlq",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            await channel.QueueBindAsync(
                queue: "catalog-products",
                exchange: _options.ExchangeName,
                routingKey: IntegrationEventNames.ProductCreatedV1,
                cancellationToken: cancellationToken);

            await channel.QueueBindAsync(
                queue: "catalog-products-retry",
                exchange: retryExchange,
                routingKey: IntegrationEventNames.ProductCreatedV1,
                cancellationToken: cancellationToken);

            await channel.QueueBindAsync(
                queue: "catalog-products-dlq",
                exchange: dlxExchange,
                routingKey: IntegrationEventNames.ProductCreatedV1,
                cancellationToken: cancellationToken);
        }
    }
}
