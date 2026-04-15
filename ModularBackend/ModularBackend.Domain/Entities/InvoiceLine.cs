using ModularBackend.Domain.Abstractions;
using ModularBackend.Domain.Exceptions;
using ModularBackend.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.Entities
{
    public sealed class InvoiceLine : Entity
    {
        public Guid? ProductId { get; private set; }
        public string Description { get; private set; }
        public decimal Quantity { get; private set; }
        public Money UnitPrice { get; private set; }
        public TaxRate TaxRate{ get; private set; }
        public decimal DiscountPercentage { get; private set; }

        public InvoiceLine()
        {
            
        }

        public InvoiceLine(string description, TaxRate taxRate , decimal quantity, Money unitPrice, Guid? productId = null, decimal discountPercentage = 0m)
        { 
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required.", nameof(description));

            if (quantity <= 0)
                throw new ArgumentOutOfRangeException("Quantity must be > 0.");

            if (discountPercentage < 0 || discountPercentage > 100)
                throw new ArgumentOutOfRangeException(nameof(discountPercentage));

            if(unitPrice is null)
                throw new ArgumentNullException(nameof(unitPrice));

            if(taxRate.Equals(default))
                throw new ArgumentException("Tax rate is required.", nameof(taxRate));

            Description = description.Trim();
            TaxRate = taxRate;
            Quantity = quantity;
            UnitPrice = unitPrice;
            ProductId = productId;
            DiscountPercentage = discountPercentage;
        }

        public Money NetSubtotal()
        {
            var gross = UnitPrice * Quantity;
            var factor = (100m - DiscountPercentage) / 100m;
            return gross * factor;
        }

        public Money Total() => NetSubtotal() + TaxAmount();

        public Money TaxAmount()
            => NetSubtotal() * TaxRate.Factor;
    }
}
