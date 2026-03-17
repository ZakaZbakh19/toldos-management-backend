using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularBackend.Application.IntegrationEvents;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ModularBackend.Infrastructure.EventBus
{

    public sealed class RabbitMqIntegrationEventBus : IIntegrationEventBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqIntegrationEventBus> _logger;

        public RabbitMqIntegrationEventBus(
            IOptions<RabbitMqOptions> options,
            ILogger<RabbitMqIntegrationEventBus> logger)
        {
            _options = options.Value;
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password
            };

            //_connection = await factory.CreateConnectionAsync();
            //_channel = _connection.CreateModel();

            //_channel.ExchangeDeclare(
            //    exchange: _options.ExchangeName,
            //    type: ExchangeType.Topic,
            //    durable: true,
            //    autoDelete: false);
        }

        public Task PublishAsync<TIntegrationEvent>(
            TIntegrationEvent integrationEvent,
            CancellationToken cancellationToken = default)
            where TIntegrationEvent : IIntegrationEvent
        {
            //var eventType = integrationEvent switch
            //{
            //    ProductCreatedIntegrationEvent => IntegrationEventNames.ProductCreatedV1,
            //    _ => throw new InvalidOperationException($"Unknown integration event type: {integrationEvent.GetType().Name}")
            //};

            //var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(integrationEvent));
            //var properties = _channel.CreateBasicProperties();

            //properties.MessageId = integrationEvent.EventId.ToString("N");
            //properties.Type = eventType;
            //properties.ContentType = "application/json";
            //properties.DeliveryMode = 2;

            //_channel.BasicPublish(
            //    exchange: _options.ExchangeName,
            //    routingKey: eventType,
            //    basicProperties: properties,
            //    body: body);

            //_logger.LogInformation(
            //    "Published event {EventType} with EventId {EventId}",
            //    eventType,
            //    integrationEvent.EventId);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            //_channel.Dispose();
            //_connection.Dispose();
        }
    }
}
