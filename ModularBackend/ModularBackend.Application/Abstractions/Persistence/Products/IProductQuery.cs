using ModularBackend.Application.Products.Queries.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Persistence.Product
{
    public interface IProductQuery
    {
        Task<ProductDetailDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<ProductDetailDTO>?> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default);
    }
}
