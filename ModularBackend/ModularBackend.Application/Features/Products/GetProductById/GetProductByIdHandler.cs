using ModularBackend.Application.Abstractions.Persistence.Products;
using ModularBackend.Application.Exceptions;
using ModularBackend.Application.Features.Products.Common;
using ModularBackend.Application.Mediator;

namespace ModularBackend.Application.Features.Products.GetProductById
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDetailDTO>
    {
        private readonly IProductQuery _productQueries;

        public GetProductByIdHandler(IProductQuery productQueries)
        {
            _productQueries = productQueries;
        }
        public async Task<ProductDetailDTO> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await _productQueries.GetByIdAsync(request.Id, cancellationToken);
            return dto ?? throw new NotFoundException($"Product with id '{request.Id}' was not found.");
        }
    }
}
