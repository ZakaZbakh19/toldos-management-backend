using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ModularBackend.Domain.Entities;
using ModularBackend.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Persistence.Configurations
{
    public sealed class CustomersConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            // Query filter global para soft delete
            builder.HasQueryFilter(x => !x.IsDeleted);

            // TaxId
            builder.OwnsOne(x => x.TaxId, taxId =>
            {
                taxId.Property(t => t.Value)
                    .HasColumnName("TaxId")
                    .HasMaxLength(50)
                    .IsRequired();
            });

            // Contact
            builder.OwnsOne(x => x.Contact, contact =>
            {
                contact.Property(c => c.Phone)
                    .HasColumnName("Phone")
                    .HasMaxLength(20)
                    .IsRequired(false);

                contact.OwnsOne(c => c.Email, email =>
                {
                    email.Property(e => e.Value)
                        .HasColumnName("Email")
                        .HasMaxLength(255)
                        .IsRequired(false);
                });
            });

            // BillingAddress
            builder.OwnsOne(x => x.BillingAddress, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("BillingAddressStreet")
                    .HasMaxLength(200)
                    .IsRequired();

                address.Property(a => a.City)
                    .HasColumnName("BillingAddressCity")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.PostalCode)
                    .HasColumnName("BillingAddressPostalCode")
                    .HasMaxLength(20)
                    .IsRequired();

                address.Property(a => a.Country)
                    .HasColumnName("BillingAddressCountry")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            // InstallationAddress
            builder.OwnsOne(x => x.InstallationAddress, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("InstallationAddressStreet")
                    .HasMaxLength(200)
                    .IsRequired();

                address.Property(a => a.City)
                    .HasColumnName("InstallationAddressCity")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.PostalCode)
                    .HasColumnName("InstallationAddressPostalCode")
                    .HasMaxLength(20)
                    .IsRequired();

                address.Property(a => a.Country)
                    .HasColumnName("InstallationAddressCountry")
                    .HasMaxLength(100)
                    .IsRequired();
            });
        }
    }
}
