using ModularBackend.Application.Features.Products.Common;
using ModularBackend.Application.Mediator;
using ModularBackend.Application.Shared;

namespace ModularBackend.Application.Features.Products.GetProducts
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
