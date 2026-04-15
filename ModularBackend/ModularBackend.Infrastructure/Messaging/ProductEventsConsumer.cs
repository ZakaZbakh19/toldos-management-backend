using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModularBackend.Application.Abstractions.Events.IntegrationEvents;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.EventBus
{
    public sealed class ProductEventsConsumer : BackgroundService
    {
        private const string ConsumerName = "catalog-products-consumer";
        private const string QueueName = "catalog-products";
        private const string RetryExchange = "integration-events-retry";
        private const string DeadLetterExchange = "integration-events-dlx";
        private const int MaxRetries = 3;

        private readonly IConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ProductEventsConsumer> _logger;

        private IChannel? _channel;

        public ProductEventsConsumer(
            IConnection connection,
            IServiceScopeFactory scopeFactory,
            ILogger<ProductEventsConsumer> logger)
        {
            _connection = connection;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 10,
                global: false,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                using var scope = _scopeFactory.CreateScope();
                var dispatcher = scope.ServiceProvider.GetRequiredService<IIntegrationEventDispatcher>();

                var messageId = ea.BasicProperties.MessageId ?? string.Empty;
                var eventType = ea.RoutingKey;
                var payload = Encoding.UTF8.GetString(ea.Body.ToArray());
                var retryCount = GetRetryCount(ea.BasicProperties, QueueName);

                try
                {
                    await dispatcher.DispatchAsync(
                        messageId,
                        eventType,
                        payload,
                        ConsumerName,
                        stoppingToken);

                    await _channel.BasicAckAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error consuming message {MessageId}. RetryCount: {RetryCount}",
                        messageId,
                        retryCount);

                    if (retryCount >= MaxRetries)
                    {
                        _logger.LogWarning(
                            "Message {MessageId} exceeded max retries. Sending to DLQ.",
                            messageId);

                        await RepublishAsync(
                            _channel,
                            ea,
                            DeadLetterExchange,
                            eventType,
                            retryCount,
                            stoppingToken);

                        await _channel.BasicAckAsync(
                            deliveryTag: ea.DeliveryTag,
                            multiple: false,
                            cancellationToken: stoppingToken);
                    }
                    else
                    {
                        await _channel.BasicNackAsync(
                            ea.DeliveryTag,
                            multiple: false,
                            requeue: false,
                            cancellationToken: stoppingToken);
                    }
                }
                finally
                {
                }
            };

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private static int GetRetryCount(IReadOnlyBasicProperties properties, string sourceQueueName)
        {
            if (properties.Headers is null ||
                !properties.Headers.TryGetValue("x-death", out var value) ||
                value is not IList<object> deaths)
            {
                return 0;
            }

            foreach (var death in deaths)
            {
                if (death is not IDictionary<string, object> entry)
                    continue;

                if (!entry.TryGetValue("queue", out var queueObj) || queueObj is not byte[] queueBytes)
                    continue;

                var queueName = Encoding.UTF8.GetString(queueBytes);

                if (!string.Equals(queueName, sourceQueueName, StringComparison.Ordinal))
                    continue;

                if (!entry.TryGetValue("count", out var countObj))
                    return 0;

                return countObj switch
                {
                    long longCount => (int)longCount,
                    int intCount => intCount,
                    byte[] bytes when int.TryParse(Encoding.UTF8.GetString(bytes), out var parsed) => parsed,
                    _ => 0
                };
            }

            return 0;
        }

        private static async Task RepublishAsync(
            IChannel channel,
            BasicDeliverEventArgs ea,
            string targetExchange,
            string routingKey,
            int retryCount,
            CancellationToken cancellationToken)
        {
            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = ea.BasicProperties.ContentType ?? "application/json",
                MessageId = ea.BasicProperties.MessageId,
                Headers = ea.BasicProperties.Headers is null
                    ? new Dictionary<string, object?>()
                    : new Dictionary<string, object?>(ea.BasicProperties.Headers)
            };

            await channel.BasicPublishAsync(
                exchange: targetExchange,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: properties,
                body: ea.Body,
                cancellationToken: cancellationToken);
        }
    }
}
