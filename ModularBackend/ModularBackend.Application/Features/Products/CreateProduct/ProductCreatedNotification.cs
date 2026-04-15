using ModularBackend.Application.Abstractions.Events.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Features.Products.CreateProduct
{
    public sealed record ProductCreatedNotification(Guid ProductId, string Name, decimal Price) : INotification;
}
