using ModularBackend.Application.Features.Products.Common;
using ModularBackend.Application.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Features.Products.GetProductById
{
    public sealed record GetProductByIdQuery(Guid Id) : IQueryRequest<ProductDetailDTO>;
}
