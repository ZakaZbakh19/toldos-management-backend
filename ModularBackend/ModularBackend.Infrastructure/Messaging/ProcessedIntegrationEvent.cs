using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Messaging
{
    public sealed class ProcessedIntegrationEvent
    {
        public Guid EventId { get; set; }
        public string ConsumerName { get; set; } = default!;
        public string EventType { get; set; } = default!;

        public string Status { get; set; } = default!; // InProgress / Processed / Failed

        public DateTime StartedOnUtc { get; set; }
        public DateTime? ProcessedOnUtc { get; set; }
        public DateTime? FailedOnUtc { get; set; }

        public string? Error { get; set; }
    }
}
