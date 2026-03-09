using ModularBackend.Domain.Abstractions;
using ModularBackend.Domain.Enumerables;
using ModularBackend.Domain.Exceptions;
using ModularBackend.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.Entities
{
    public sealed class Invoice : AggregateRoot
    {
        public Guid CustomerId { get; private set; }
        public InvoiceStatusType Status { get; private set; }
        public InvoiceNumber? Number { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? IssuedAt { get; private set; }
        public DateTime? PaidAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }
        public CurrencyType Currency { get; private set; }

        private readonly List<InvoiceLine> _lines = new();
        public IReadOnlyCollection<InvoiceLine> Lines => _lines;

        public Invoice(Guid customerId, CurrencyType currency)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("CustomerId is required.", nameof(customerId));

            if(!Enum.IsDefined(typeof(CurrencyType), currency))
                throw new ArgumentException("Invalid currency.", nameof(currency));

            CustomerId = customerId;
            Status = InvoiceStatusType.Draft;
            CreatedAt = DateTime.UtcNow;
            Currency = currency;
        }

        public void AddProductLine(Product product, decimal quantity, decimal discount = 0m)
        {
            EnsureDraft();
            EnsureNotCancelled();

            var line = new InvoiceLine(
                description: product.Name,
                taxRate: product.TaxRate,
                quantity: quantity,
                unitPrice: product.BasePrice,
                productId: product.Id,
                discountPercentage: discount);

            _lines.Add(line);
        }


        public void RemoveProductLine(Guid lineId)
        {
            if (lineId == Guid.Empty)
                throw new ArgumentException("LineId is required.", nameof(lineId));

            EnsureDraft();
            EnsureNotCancelled();

            var line = _lines.Find(x => x.Id == lineId);
            if (line is null)
                throw new BusinessRuleViolationException("Invoice line not found.");

            _lines.Remove(line);
        }

        public void Issue(InvoiceNumber number)
        {
            if (number is null) throw new ArgumentNullException(nameof(number));

            EnsureDraft();
            EnsureNotCancelled();

            if (_lines.Count == 0)
                throw new BusinessRuleViolationException("Cannot issue an invoice without lines.");

            if (Number is not null)
                throw new BusinessRuleViolationException("Invoice already has a number.");

            Number = number;
            Status = InvoiceStatusType.Issued;
            IssuedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            EnsureNotCancelled();

            if (Status == InvoiceStatusType.Paid)
                throw new BusinessRuleViolationException("Cannot cancel a paid invoice.");

            Status = InvoiceStatusType.Cancelled;
            CancelledAt = DateTime.UtcNow;
        }

        public void MarkAsPaid()
        {
            EnsureNotCancelled();

            if (Status != InvoiceStatusType.Issued)
                throw new BusinessRuleViolationException("Only issued invoices can be paid.");

            Status = InvoiceStatusType.Paid;
            PaidAt = DateTime.UtcNow;
        }

        public (Money Subtotal, Money Tax, Money Total) Totals()
        {
            var subtotal = Money.Zero(Currency);
            var tax = Money.Zero(Currency);

            foreach (var line in _lines)
            {
                subtotal += line.NetSubtotal();
                tax += line.TaxAmount();
            }

            return (subtotal, tax, subtotal + tax);
        }

        private void EnsureDraft()
        {
            if (Status != InvoiceStatusType.Draft)
                throw new BusinessRuleViolationException("Only draft invoices can be modified.");
        }

        private void EnsureNotCancelled()
        {
            if (Status == InvoiceStatusType.Cancelled)
                throw new BusinessRuleViolationException("Cancelled invoice cannot be modified.");
        }

        private Invoice() { }
    }
}
