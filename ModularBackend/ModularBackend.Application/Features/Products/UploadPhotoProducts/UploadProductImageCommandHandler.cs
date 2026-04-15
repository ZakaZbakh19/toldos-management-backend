using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Application.Abstractions.Persistence.Products;
using ModularBackend.Application.Abstractions.Services.Cache;
using ModularBackend.Application.Abstractions.Services.Files;
using ModularBackend.Application.Exceptions;
using ModularBackend.Application.Mediator;
using ModularBackend.Domain.Entities;

namespace ModularBackend.Application.Features.Products.UploadPhotoProducts
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
            var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken);

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

                        product.AddPhoto(
                            photoId: photoId,
                            storageKey: stored.StorageKey,
                            contentType: stored.ContentType,
                            contentHash: stored.ContentHash,
                            contentLength: stored.ContentLength,
                            originalFileName: file.FileName
                        );

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
