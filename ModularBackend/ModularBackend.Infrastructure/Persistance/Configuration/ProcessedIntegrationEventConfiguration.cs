using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModularBackend.Infrastructure.EventBus;
using ModularBackend.Infrastructure.Outbox;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Persistance.Configuration
{
    public sealed class ProcessedIntegrationEventConfiguration : IEntityTypeConfiguration<ProcessedIntegrationEvent>
    {
        public void Configure(EntityTypeBuilder<ProcessedIntegrationEvent> builder)
        {
            builder.ToTable("ProcessedIntegrationEvent");
            builder.HasKey(x => x.EventId);
        }
    }
}
