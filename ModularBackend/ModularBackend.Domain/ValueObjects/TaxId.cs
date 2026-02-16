using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.ValueObjects
{
    public readonly record struct TaxId
    {
        public string Value { get; }

        public TaxId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Tax ID cannot be null or empty.", nameof(value));

            Value = value.Trim().ToUpperInvariant();
        }
    }
}
