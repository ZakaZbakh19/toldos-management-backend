using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Application.Abstractions.Persistence.Product;
using ModularBackend.Application.Abstractions.Persistence.Products;
using ModularBackend.Application.Exceptions;
using ModularBackend.Application.Products.Commands.CreateProduct;
using ModularBackend.Domain.Entities;
using ModularBackend.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Commands.UploadPhotoProducts
{
    public class UploadProductImageCommandHandler : IRequestHandler<UploadProductPhotosCommand, IReadOnlyCollection<string>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductQuery _productQuery;
        private readonly IUnitOfWork _unitOfWork;

        public UploadProductImageCommandHandler(IProductRepository productRepository, 
            IProductQuery productQuery,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _productQuery = productQuery;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyCollection<string>> Handle(UploadProductPhotosCommand command, CancellationToken cancellationToken)
        {
            var product = await _productQuery.GetByIdAsync(command.ProductId, cancellationToken);

            if(product == null)
                throw new NotFoundException($"Product with id '{command.ProductId}' was not found.");

            var currentOrder = product.Photos.Count;

            foreach (var file in command.Files)
            {
                var stored = await _photoStorageService.UploadAsync(
                    file.Content,
                    file.FileName,
                    file.ContentType,
                    ct);

                product.AddPhoto(stored.Url, ++currentOrder);
            }

            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitAsync(ct);

        }
    }
}
