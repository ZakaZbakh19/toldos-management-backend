using ModularBackend.Application.Abstractions.Events.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Features.Products.CreateProduct
{
    public sealed record ProductCreatedIntegrationEvent(
        Guid EventId,
        DateTime OccurredOnUtc,
        Guid ProductId,
        string Name,
        decimal Price) : IIntegrationEvent;
}
