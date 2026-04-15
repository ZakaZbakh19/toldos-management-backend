using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModularBackend.Domain.Entities;
using ModularBackend.Infrastructure.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Persistence.Configurations
{
    public sealed class ProductFilesConfiguration : IEntityTypeConfiguration<ProductFile>
    {
        public void Configure(EntityTypeBuilder<ProductFile> builder)
        {
            builder.ToTable("ProductFiles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.StorageKey)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.ContentHash)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(x => x.ContentType)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.ContentLength)
                .IsRequired();

            builder.Property(x => x.OriginalFileName)
                .HasMaxLength(255);

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.HasIndex(x => new { x.StorageKey })
                .IsUnique();

            builder.HasIndex(x => new { x.ProductId, x.ContentHash })
                .IsUnique();
        }
    }
}
