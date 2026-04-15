using Microsoft.Extensions.Logging;
using ModularBackend.Application.Abstractions.Events.Notifications;

namespace ModularBackend.Application.Features.Products.CreateProduct
{
    public sealed class ProductCreatedHandler
        : INotificationHandler<ProductCreatedNotification>
    {
        private readonly ILogger<ProductCreatedHandler> _log;
        public ProductCreatedHandler(ILogger<ProductCreatedHandler> log) { _log = log; }
        public Task Handle(ProductCreatedNotification notification, CancellationToken ct)
        {
            _log.LogInformation(
              "Product created. Product name: {Name}, Price: {Price}, ProductId: {ProductId}",
              notification.Name,
              notification.Price,
              notification.ProductId);

            return Task.CompletedTask;
        }
    }
}
