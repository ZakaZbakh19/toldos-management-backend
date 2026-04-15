using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Features.Products.Common
{
    public sealed record ProductDetailDTO(Guid Id, string Name, string Description, decimal Price);
}
