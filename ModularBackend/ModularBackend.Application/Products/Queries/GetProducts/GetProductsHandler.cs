using ModularBackend.Application.Abstractions.Common;
using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Abstractions.Persistence.Product;
using ModularBackend.Application.Exceptions;
using ModularBackend.Application.Products.Queries.Common;
using ModularBackend.Application.Products.Queries.GetProductById;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Queries.GetProducts
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
            var result = await _productQuery.GetPagedAsync(request, cancellationToken);
            return result ?? throw new NotFoundException($"Products not founded.");
        }
    }
}
