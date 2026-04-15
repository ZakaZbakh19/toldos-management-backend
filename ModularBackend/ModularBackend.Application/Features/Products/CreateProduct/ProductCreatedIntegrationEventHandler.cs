using Microsoft.Extensions.Logging;
using ModularBackend.Application.Abstractions.Events.IntegrationEvents;

namespace ModularBackend.Application.Features.Products.CreateProduct
{
    public sealed class ProductCreatedIntegrationEventHandler
        : IIntegrationEventHandler<ProductCreatedIntegrationEvent>
    {
        private readonly ILogger<ProductCreatedIntegrationEventHandler> _logger;

        public ProductCreatedIntegrationEventHandler(
            ILogger<ProductCreatedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(
            ProductCreatedIntegrationEvent integrationEvent,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Handling ProductCreatedIntegrationEvent. ProductId: {ProductId}, Name: {Name}, Price: {Price}",
                integrationEvent.ProductId,
                integrationEvent.Name,
                integrationEvent.Price);

            // Aquí iría la lógica real:
            // - actualizar una proyección
            // - crear un registro en otra tabla
            // - llamar a otro módulo
            // - enviar notificación
            // etc.

            return Task.CompletedTask;
        }
    }
}
