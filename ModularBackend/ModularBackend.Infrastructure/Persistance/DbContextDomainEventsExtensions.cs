using Microsoft.EntityFrameworkCore;
using ModularBackend.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Persistance
{
    public static class DbContextDomainEventsExtensions
    {
        public static IReadOnlyCollection<IDomainEvent> CollectDomainEvents(this DbContext dbContext, bool deleteDomainsEvents = false)
        {
            var aggregates = dbContext.ChangeTracker
                .Entries<AggregateRoot>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = aggregates
                .SelectMany(a => a.DomainEvents)
                .ToList();

            if(deleteDomainsEvents)
                aggregates.ForEach(a => a.ClearDomainEvents());

            return domainEvents;
        }
    }
}
