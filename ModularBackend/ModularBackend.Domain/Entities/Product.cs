using ModularBackend.Domain.Abstractions;
using ModularBackend.Domain.Events;
using ModularBackend.Domain.Exceptions;
using ModularBackend.Domain.ValueObjects;

namespace ModularBackend.Domain.Entities
{
    public sealed class Product : AggregateRoot
    {
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

        public void AddPhoto(ProductFile resource)
        {
            EnsureActiveForModification();

            if (_photos.Count >= MaxPhotos)
                throw new BusinessRuleViolationException("The product cannot contain more than 10 photos.");

            if (_photos.Any(x => x.ContentHash == resource.ContentHash))
                throw new BusinessRuleViolationException("The same photo has already been added to this product.");

            if (resource.ContentType == "image/gif")
                throw new BusinessRuleViolationException("GIFs not allowed");

            _photos.Add(ProductFile.Create(
                id: resource.Id,
                productId: Id,
                storageKey: resource.StorageKey,
                contentType: resource.ContentType,
                contentHash: resource.ContentHash,
                contentLength: resource.ContentLength,
                originalFileName: resource.OriginalFileName
            ));
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
