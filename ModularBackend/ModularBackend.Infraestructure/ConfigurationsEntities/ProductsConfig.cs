using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModularBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infraestructure.ConfigurationsEntities
{
    public class ProductsConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.OwnsOne(p => p.BasePrice, money =>
            {
                money.Property(m => m.Amount)
                     .HasPrecision(18, 2)
                     .IsRequired();

                money.Property(m => m.Currency)
                     .IsRequired();
            });
        }
    }
}
