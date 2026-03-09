using ModularBackend.Application.Products.Queries.GetProductById;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Persistence.Product
{
    public interface IProductQuery
    {
        Task<ProductDetailDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
