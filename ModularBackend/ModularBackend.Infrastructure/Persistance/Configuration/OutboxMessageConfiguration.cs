using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModularBackend.Infrastructure.Outbox;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Persistance.Configuration
{
    public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Payload).IsRequired();
            builder.Property(x => x.OccurredOnUtc).IsRequired();
            builder.Property(x => x.ProcessedOnUtc);
            builder.Property(x => x.Error).HasMaxLength(4000);
            builder.Property(x => x.Attempts).IsRequired();
            builder.Property(x => x.LastAttemptOnUtc);

            builder.HasIndex(x => new { x.ProcessedOnUtc, x.OccurredOnUtc, x.LastAttemptOnUtc });
        }
    }
}
