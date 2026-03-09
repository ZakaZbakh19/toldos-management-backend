using ModularBackend.Domain.Abstractions;
using ModularBackend.Domain.Exceptions;
using ModularBackend.Domain.ValueObjects;

namespace ModularBackend.Domain.Entities
{
    public sealed class Product : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public Money BasePrice { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }
        public TaxRate TaxRate { get; private set; }

        public Product()
        {
            
        }

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
            if(newBasePrice is null) throw new ArgumentException("New base price is required.", nameof(newBasePrice));
            EnsureActiveForModification();

            BasePrice = newBasePrice;
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty.", nameof(name));

            EnsureActiveForModification();

            Name = name.Trim();
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
