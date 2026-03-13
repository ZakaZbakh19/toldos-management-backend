using ModularBackend.Domain.Abstractions;

namespace ModularBackend.Domain.Events
{
    public record UserRegistedEvent(string name, string email) : IDomainEvent
    {
        public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
    }
}
