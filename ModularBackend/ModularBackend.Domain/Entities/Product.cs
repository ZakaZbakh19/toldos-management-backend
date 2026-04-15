using ModularBackend.Domain.Abstractions;
using ModularBackend.Domain.Events;
using ModularBackend.Domain.Exceptions;
using ModularBackend.Domain.ValueObjects;

namespace ModularBackend.Domain.Entities
{
    public sealed class Product : AggregateRoot
    {
        private static readonly HashSet<string> AllowedContentTypes = new()
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/webp"
        };

        private const int MaxPhotos = 10;
        public string Name { get; private set; } = string.Empty;
        public Money BasePrice { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }
        public TaxRate TaxRate { get; private set; }

        private readonly List<ProductFile> _photos = new();
        public IReadOnlyCollection<ProductFile> Photos => _photos.AsReadOnly();

        private Product(){ }

        public Product(string name, TaxRate taxRate, Money basePrice, string? description = null, bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty.", nameof(name));

            if (basePrice == null)
                throw new ArgumentException("Base price is required.", nameof(basePrice));

            if (description is not null && description.Length > 500)
                throw new ArgumentException("Product description cannot exceed 500 characters.", nameof(description));

            Name = name.Trim();
            TaxRate = taxRate;
            BasePrice = basePrice;
            Description = description?.Trim() ?? string.Empty;
            IsActive = isActive;

            AddDomainEvent(new ProductCreatedDomainEvent(Id, Name, BasePrice.Amount));
        }

        public void ChangeDescription(string? description)
        {
            EnsureActiveForModification();
            if (description is not null && description.Length > 500)
                throw new ArgumentException("Product description cannot exceed 500 characters.", nameof(description));
            Description = description?.Trim() ?? string.Empty;
        }

        public void ChangeTaxRate(TaxRate newTaxRate)
        {
            EnsureActiveForModification();
            TaxRate = newTaxRate;
        }

        public void ChangePrice(Money newBasePrice)
        {
            EnsureActiveForModification();
            if (newBasePrice is null) throw new ArgumentException("New base price is required.", nameof(newBasePrice));

            BasePrice = newBasePrice;
        }

        public void Rename(string name)
        {
            EnsureActiveForModification();

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty.", nameof(name));

            Name = name.Trim();
        }

        public void AddPhoto(Guid photoId,
            string storageKey,
            string contentType,
            string contentHash,
            long contentLength,
            string? originalFileName)
        {
            EnsureActiveForModification();

            if(string.IsNullOrWhiteSpace(storageKey))
                throw new ArgumentException("Storage key cannot be empty.", nameof(storageKey));

            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("Content type cannot be empty.", nameof(contentType));

            if (!AllowedContentTypes.Any(x => x == contentType))
                throw new BusinessRuleViolationException("This content type is not allowed.");

            if (_photos.Count >= MaxPhotos)
                throw new BusinessRuleViolationException("The product cannot contain more than 10 photos.");

            if (_photos.Any(x => x.ContentHash == contentHash))
                throw new BusinessRuleViolationException("The same photo has already been added to this product.");

            var photo = ProductFile.Create(
                id: photoId,
                productId: this.Id,
                storageKey: storageKey,
                contentType: contentType,
                contentHash: contentHash,
                contentLength: contentLength,
                originalFileName: originalFileName
            );

            _photos.Add(photo);
        }

        public void RemovePhoto(Guid photoId)
        {
            EnsureActiveForModification();

            var photo = _photos.SingleOrDefault(x => x.Id == photoId)
                ?? throw new BusinessRuleViolationException("Photo not found.");

            _photos.Remove(photo);
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;

        private void EnsureActiveForModification()
        {
            if (!IsActive)
                throw new BusinessRuleViolationException("Inactive products cannot be modified.");
        }
    }
}
