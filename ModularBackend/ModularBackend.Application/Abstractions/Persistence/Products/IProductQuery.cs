using ModularBackend.Application.Features.Products.Common;
using ModularBackend.Application.Features.Products.GetProducts;
using ModularBackend.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Persistence.Products
{
    public interface IProductQuery
    {
        Task<ProductDetailDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<ProductDetailDTO>> GetPagedAsync(GetProductsQuery request, CancellationToken cancellationToken = default);
    }
}
