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
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductDetailDTO>>
    {
        private readonly IProductQuery _productQueries;

        public GetProductsHandler(IProductQuery productQueries)
        {
            _productQueries = productQueries;
        }
        public async Task<PagedResult<ProductDetailDTO>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            
        }
    }
}
