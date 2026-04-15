using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModularBackend.Infrastructure.EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Persistence.Configurations
{
    public sealed class ProcessedIntegrationEventConfiguration
        : IEntityTypeConfiguration<ProcessedIntegrationEvent>
    {
        public void Configure(EntityTypeBuilder<ProcessedIntegrationEvent> builder)
        {
            builder.ToTable("ProcessedIntegrationEvents");
            builder.HasKey(x => new { x.EventId, x.ConsumerName });

            builder.Property(x => x.ConsumerName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.EventType)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(x => x.ProcessedOnUtc)
                .IsRequired();
        }
    }
}
