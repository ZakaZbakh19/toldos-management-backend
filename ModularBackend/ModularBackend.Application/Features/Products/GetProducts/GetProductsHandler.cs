using ModularBackend.Application.Abstractions.Persistence.Products;
using ModularBackend.Application.Exceptions;
using ModularBackend.Application.Features.Products.Common;
using ModularBackend.Application.Mediator;
using ModularBackend.Application.Shared;

namespace ModularBackend.Application.Features.Products.GetProducts
{
    public sealed class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductDetailDTO>>
    {
        private readonly IProductQuery _productQuery;

        public GetProductsHandler(IProductQuery productQuery)
        {
            _productQuery = productQuery;
        }

        public async Task<PagedResult<ProductDetailDTO>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            return await _productQuery.GetPagedAsync(request, cancellationToken);
        }
    }
}
