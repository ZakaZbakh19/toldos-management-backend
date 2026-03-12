using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Commands.CreateProduct
{
    public sealed record CreateProductDTO(Guid Id, string Name, string Description, decimal Price);
}
