using ModularBackend.Application.Abstractions.Common;
using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Products.Queries.Common;
using ModularBackend.Application.Products.Queries.GetProductById;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Queries.GetProducts
{
    public sealed record GetProductsQuery(
        int Page,
        int PageSize,
        string? Search,
        bool? IsActive,
        decimal? MinPrice,
        decimal? MaxPrice,
        string? SortBy,
        bool Desc
    ) : IQueryRequest<PagedResult<ProductDetailDTO>>;
}
