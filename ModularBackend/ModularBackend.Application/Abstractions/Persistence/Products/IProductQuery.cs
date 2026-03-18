using ModularBackend.Application.Abstractions.Common;
using ModularBackend.Application.Products.Queries.Common;
using ModularBackend.Application.Products.Queries.GetProducts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Persistence.Product
{
    public interface IProductQuery
    {
        Task<ProductDetailDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<ProductDetailDTO>> GetPagedAsync(GetProductsQuery request, CancellationToken cancellationToken = default);
    }
}
