using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.EventBus
{
    public sealed class ProcessedIntegrationEvent
    {
        public Guid EventId { get; set; }
        public string ConsumerName { get; set; } = default!;
        public string EventType { get; set; } = default!;
        public DateTime ProcessedOnUtc { get; set; }
    }
}
