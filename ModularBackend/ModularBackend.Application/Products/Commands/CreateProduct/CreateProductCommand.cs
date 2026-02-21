using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Domain.Enumerables;
using ModularBackend.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Commands.CreateProduct
{
    public sealed record CreateProductCommand(
        string Name,
        decimal BasePrice,
        decimal TaxRate,
        string Description,
        CurrencyType Currency,
        bool IsActive = false
    ) : IRequest<Guid>;
}
