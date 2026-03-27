using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Application.Abstractions.Persistence.Product;
using ModularBackend.Application.Abstractions.Persistence.Products;
using ModularBackend.Application.Cache;
using ModularBackend.Application.Exceptions;
using ModularBackend.Application.Products.Commands.CreateProduct;
using ModularBackend.Application.Services;
using ModularBackend.Domain.Entities;
using ModularBackend.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Commands.UploadPhotoProducts
{
    public class UploadProductImageCommandHandler : IRequestHandler<UploadProductPhotosCommand, IReadOnlyCollection<UploadedPhotoDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IStorageKeyFactory _storageKeyFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheInvalidator _cacheInvalidator;

        public UploadProductImageCommandHandler(IProductRepository productRepository,
            IFileStorageService fileStorageService,
            IStorageKeyFactory storageKeyFactory,
            IUnitOfWork unitOfWork,
            ICacheInvalidator cacheInvalidator)
        {
            _productRepository = productRepository;
            _fileStorageService = fileStorageService;
            _storageKeyFactory = storageKeyFactory;
            _unitOfWork = unitOfWork;
            _cacheInvalidator = cacheInvalidator;
        }

        public async Task<IReadOnlyCollection<UploadedPhotoDto>> Handle(UploadProductPhotosCommand command, CancellationToken cancellationToken)
        {
            var listUrlImages = new List<string>();
            var product = await _productRepository.GetByByIdAsync(command.ProductId, cancellationToken);

            if (product == null)
                throw new NotFoundException($"Product with id '{command.ProductId}' was not found.");

            var uploaded = new List<UploadedPhotoDto>();
            var uploadedKeys = new List<string>();

            try
            {
                foreach (var file in command.Files)
                {
                    var photoId = Guid.NewGuid();
                    var extension = Path.GetExtension(file.FileName);

                    var storageKey = _storageKeyFactory
                        .CreateProductPhotoOriginal(product.Id, photoId, extension);

                    try
                    {
                        var stored = await _fileStorageService.UploadAsync(
                            new FileUploadRequest(
                                storageKey,
                                file.Content,
                                file.ContentType,
                                file.Length,
                                file.FileName),
                            cancellationToken);

                        uploadedKeys.Add(storageKey);

                        product.AddPhoto(ProductFile.Create
                        (
                            photoId,
                            product.Id,
                            stored.StorageKey,
                            stored.ContentHash,
                            stored.ContentType,
                            stored.ContentLength,
                            file.FileName
                        ));

                        uploaded.Add(new UploadedPhotoDto(photoId, storageKey));
                    }
                    catch
                    {
                        await _fileStorageService.DeleteIfExistsAsync(storageKey, cancellationToken);
                        throw;
                    }
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _cacheInvalidator.InvalidateProductsAsync(cancellationToken);

                return uploaded.AsReadOnly();
            }
            catch
            {
                foreach (var key in uploadedKeys)
                    await _fileStorageService.DeleteIfExistsAsync(key, cancellationToken);

                throw;
            }
        }
    }
}
