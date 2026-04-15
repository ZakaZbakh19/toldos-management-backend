using Microsoft.EntityFrameworkCore;
using ModularBackend.Application.Abstractions.Services.Messaging;
using ModularBackend.Infrastructure.Persistence;

namespace ModularBackend.Infrastructure.EventBus
{
    public sealed class ProcessedIntegrationEventStore : IProcessedIntegrationEventStore
    {
        private readonly ApplicationDbContext _dbContext;

        public ProcessedIntegrationEventStore(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> HasBeenProcessedAsync(
            Guid eventId,
            string consumerName,
            CancellationToken cancellationToken)
        {
            return _dbContext.ProcessedIntegrationEvents
                .AnyAsync(
                    x => x.EventId == eventId && x.ConsumerName == consumerName,
                    cancellationToken);
        }

        public async Task MarkAsProcessedAsync(
            Guid eventId,
            string consumerName,
            string eventType,
            CancellationToken cancellationToken)
        {
            _dbContext.ProcessedIntegrationEvents.Add(new ProcessedIntegrationEvent
            {
                EventId = eventId,
                ConsumerName = consumerName,
                EventType = eventType,
                ProcessedOnUtc = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
