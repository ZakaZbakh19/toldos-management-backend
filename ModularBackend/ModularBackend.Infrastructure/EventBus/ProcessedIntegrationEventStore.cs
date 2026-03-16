using Microsoft.EntityFrameworkCore;
using ModularBackend.Application.IntegrationEvents;
using ModularBackend.Infrastructure.Persistance.Context;

namespace ModularBackend.Infrastructure.EventBus
{
    public sealed class ProcessedIntegrationEventStore : IProcessedIntegrationEventStore
    {
        private readonly ApplicationDbContext _dbContext;

        public ProcessedIntegrationEventStore(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> HasBeenProcessedAsync(Guid eventId, CancellationToken cancellationToken)
        {
            return _dbContext.ProcessedIntegrationEvents
                .AnyAsync(x => x.EventId == eventId, cancellationToken);
        }

        public async Task MarkAsProcessedAsync(Guid eventId, string eventType, CancellationToken cancellationToken)
        {
            _dbContext.ProcessedIntegrationEvents.Add(new ProcessedIntegrationEvent
            {
                EventId = eventId,
                EventType = eventType,
                ProcessedOnUtc = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
