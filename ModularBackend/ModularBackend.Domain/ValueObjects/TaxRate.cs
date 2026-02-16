using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.ValueObjects
{
    public readonly record struct TaxRate
    {
        public decimal Percentage { get; }

        public TaxRate(decimal percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentOutOfRangeException(nameof(percentage));

            Percentage = percentage;
        }

        public decimal Factor => Percentage / 100m;
    }

}
