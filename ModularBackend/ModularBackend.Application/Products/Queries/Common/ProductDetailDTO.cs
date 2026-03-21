using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Queries.Common
{
    public sealed record ProductDetailDTO(Guid Id, string Name, string Description, decimal Price);
}
