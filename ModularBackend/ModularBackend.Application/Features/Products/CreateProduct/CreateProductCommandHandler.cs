using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Application.Abstractions.Persistence.Products;
using ModularBackend.Application.Abstractions.Services.Cache;
using ModularBackend.Application.Mediator;
using ModularBackend.Domain.Entities;
using ModularBackend.Domain.ValueObjects;

namespace ModularBackend.Application.Features.Products.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductDTO>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheInvalidator _cacheInvalidator;

        public CreateProductCommandHandler(IProductRepository productRepository, 
            IUnitOfWork unitOfWork,
            ICacheInvalidator cacheInvalidator)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _cacheInvalidator = cacheInvalidator;
        }

        public async Task<CreateProductDTO> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var product = new Product(
                command.Name,
                new TaxRate(command.TaxRate),
                new Money(command.BasePrice, command.Currency),
                command.Description,
                command.IsActive
            );

            await _productRepository.AddAsync(product, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            //await _cacheInvalidator.InvalidateProductsAsync(cancellationToken);

            return new CreateProductDTO(Id: product.Id,
                Name: product.Name,
                Description: product.Description,
                Price: product.BasePrice.Amount);
        }
    }
}
