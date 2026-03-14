using ModularBackend.Application.Abstractions.Events;
using ModularBackend.Application.Products.Events;
using ModularBackend.Domain.Abstractions;
using ModularBackend.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Persistance
{
    public static class EventsMaps
    {
        public static INotification Map(IDomainEvent domainEvent) =>
            domainEvent switch
            {
                ProductCreatedDomainEvent e => new ProductCreatedNotification(e.ProductId, e.Name, e.Price),
                _ => throw new InvalidOperationException($"No notification mapping defined for {domainEvent.GetType().Name}")
            };
    }
}
