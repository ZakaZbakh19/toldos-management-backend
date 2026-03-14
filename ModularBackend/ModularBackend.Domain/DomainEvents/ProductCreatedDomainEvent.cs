using ModularBackend.Domain.Abstractions;

namespace ModularBackend.Domain.Events
{
    public sealed record ProductCreatedDomainEvent(Guid ProductId, string Name, decimal Price) : IDomainEvent
    {
        public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
    }
}
