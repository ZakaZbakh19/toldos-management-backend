using ModularBackend.Domain.Abstractions;
using ModularBackend.Domain.DomainEvents;

namespace ModularBackend.Domain.Events
{
    public sealed record ProductCreatedDomainEvent(Guid ProductId, string Name, decimal Price) : IDomainEvent
    {
        public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
        public string Type => DomainEventNames.ProductCreatedDomainEvent;
    }
}
