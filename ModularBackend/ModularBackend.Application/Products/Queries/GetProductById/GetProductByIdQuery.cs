using ModularBackend.Application.Abstractions.Messaging.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Queries.GetProductById
{
    public sealed record GetProductByIdQuery(Guid Id) : IQueryRequest<ProductDetailDTO>;
}
