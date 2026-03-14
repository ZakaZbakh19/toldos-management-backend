using ModularBackend.Application.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Events
{
    public sealed record ProductCreatedNotification(Guid ProductId, string Name, decimal Price) : INotification;
}
