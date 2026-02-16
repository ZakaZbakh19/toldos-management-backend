using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.ValueObjects
{
    public sealed record InvoiceNumber
    {
        public string Series { get; }
        public int SequentialNumber { get; }

        public InvoiceNumber(string series, int sequentialNumber)
        {
            if (string.IsNullOrWhiteSpace(series))
                throw new ArgumentException("Series cannot be empty.", nameof(series));

            if (sequentialNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(sequentialNumber));

            Series = series.Trim().ToUpperInvariant();
            SequentialNumber = sequentialNumber;
        }

        public override string ToString()
            => $"{Series}-{SequentialNumber:D6}";
    }

}
