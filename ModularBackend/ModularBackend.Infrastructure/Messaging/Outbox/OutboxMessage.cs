using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Messaging.Outbox
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = default!;
        public string Payload { get; set; } = default!;
        public DateTime OccurredOnUtc { get; set; }
        public DateTime? ProcessedOnUtc { get; set; }
        public string? Error { get; set; }
        public int Attempts { get; set; }
        public DateTime? LastAttemptOnUtc { get; set; }
    }
}
