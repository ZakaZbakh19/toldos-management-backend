using ModularBackend.Application.Abstractions.Common;
using ModularBackend.Application.Products.Queries.GetProductById;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Persistence
{
    public interface IProductReadRepository
        : IReadRepository<ProductDetailDTO>
    {
    }
}
