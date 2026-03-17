using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Products.Queries.Common;
using ModularBackend.Application.Products.Queries.GetProductById;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Queries.GetProducts
{
    public sealed record GetProductsQuery(int page, int size) : IQueryRequest<PagedResult<ProductDetailDTO>>;
}
