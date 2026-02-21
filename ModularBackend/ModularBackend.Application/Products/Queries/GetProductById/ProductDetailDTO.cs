using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Queries.GetProductById
{
    public sealed record ProductDetailDTO(Guid Id, string Name, string Description, decimal Price);
}
