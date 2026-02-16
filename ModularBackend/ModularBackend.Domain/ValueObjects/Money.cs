using ModularBackend.Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.ValueObjects
{
    public record Money
    {
        public decimal Amount { get; }
        public CurrencyType Currency { get; }

        public Money(decimal amount, CurrencyType currency)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");

            if (!Enum.IsDefined(typeof(CurrencyType), currency))
                throw new ArgumentException("Invalid currency.", nameof(currency));

            Amount = amount;
            Currency = currency;
        }

        public static Money Zero(CurrencyType currency)
            => new(0m, currency);

        public static Money operator +(Money a, Money b)
        {
            EnsureSameCurrency(a, b);
            return new Money(a.Amount + b.Amount, a.Currency);
        }

        public static Money operator *(Money money, decimal factor)
        {
            if (factor < 0)
                throw new ArgumentOutOfRangeException(nameof(factor));

            return new Money(money.Amount * factor, money.Currency);
        }

        private static void EnsureSameCurrency(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException("Cannot operate on different currencies.");
        }

        public override string ToString() => $"{Amount:0.00} {Currency}";
    }
}
