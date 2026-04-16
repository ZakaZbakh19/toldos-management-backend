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

            const string retryExchange = MessagingTopology.RetryExchange;
            const string dlxExchange = MessagingTopology.DeadLetterExchange;

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
                queue: MessagingTopology.ProductsQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object?>
                {
                    { "x-dead-letter-exchange", retryExchange }
                },
                cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: MessagingTopology.ProductsQueueRetry,
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
                queue: MessagingTopology.ProductsQueueDeadLetter,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            foreach (var eventName in IntegrationEventNames.AllEventNames)
            {
                await channel.QueueBindAsync(MessagingTopology.ProductsQueue, _options.ExchangeName, eventName, cancellationToken: cancellationToken);
                await channel.QueueBindAsync(MessagingTopology.ProductsQueueRetry, retryExchange, eventName, cancellationToken: cancellationToken);
                await channel.QueueBindAsync(MessagingTopology.ProductsQueueDeadLetter, dlxExchange, eventName, cancellationToken:cancellationToken);
            }
        }
    }
}
