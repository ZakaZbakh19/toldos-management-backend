using ModularBackend.Application.Abstractions.Events.Notifications;
using ModularBackend.Application.Features.Products.CreateProduct;
using ModularBackend.Domain.Abstractions;
using ModularBackend.Domain.Events;

namespace ModularBackend.Infrastructure.Persistence
{
    public static class NotificationsMaps
    {
        public static INotification Map(IDomainEvent domainEvent) =>
            domainEvent switch
            {
                ProductCreatedDomainEvent e => new ProductCreatedNotification(e.ProductId, e.Name, e.Price),
                _ => throw new InvalidOperationException($"No notification mapping defined for {domainEvent.GetType().Name}")
            };
    }
}
