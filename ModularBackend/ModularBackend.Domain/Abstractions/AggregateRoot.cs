using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.Abstractions
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<IDomainEvent> _events = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _events;
        protected void AddDomainEvent(IDomainEvent @event) => _events.Add(@event);
        public void ClearDomainEvents() => _events.Clear();

    }
}
