using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ModularBackend.Domain.Entities;
using ModularBackend.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Persistance
{
    public class ProductsConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.OwnsOne(p => p.BasePrice, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("BasePriceAmount")
                     .HasPrecision(18, 2)
                     .IsRequired();

                money.Property(m => m.Currency)
                     .HasColumnName("BasePriceCurrency")
                     .HasConversion<string>()
                     .HasMaxLength(3)
                     .IsRequired();
            });

            var taxRateConverter = new ValueConverter<TaxRate, decimal>(
                        v => v.Percentage,
                        v => new TaxRate(v)
                    );

            builder.Property(p => p.TaxRate)
                   .HasConversion(taxRateConverter)
                   .HasPrecision(5, 2)
                   .IsRequired();
        }
    }
}
