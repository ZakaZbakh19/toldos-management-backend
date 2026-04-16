using Microsoft.EntityFrameworkCore;
using ModularBackend.Application.Abstractions.Services.Messaging;
using ModularBackend.Infrastructure.Persistence;
using System.Data.Common;

namespace ModularBackend.Infrastructure.Messaging
{
    public sealed class ProcessedIntegrationEventStore : IProcessedIntegrationEventStore
    {
        private const string InProgress = "InProgress";
        private const string Processed = "Processed";
        private const string Failed = "Failed";

        private readonly ApplicationDbContext _dbContext;

        public ProcessedIntegrationEventStore(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> TryStartProcessingAsync(
            Guid eventId,
            string consumerName,
            string eventType,
            CancellationToken cancellationToken)
        {
            var record = new ProcessedIntegrationEvent
            {
                EventId = eventId,
                ConsumerName = consumerName,
                EventType = eventType,
                Status = InProgress,
                StartedOnUtc = DateTime.UtcNow
            };

            _dbContext.ProcessedIntegrationEvents.Add(record);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                _dbContext.Entry(record).State = EntityState.Detached;
                return false;
            }
        }

        public async Task MarkAsProcessedAsync(
            Guid eventId,
            string consumerName,
            CancellationToken cancellationToken)
        {
            var record = await _dbContext.ProcessedIntegrationEvents
                .SingleOrDefaultAsync(
                    x => x.EventId == eventId && x.ConsumerName == consumerName,
                    cancellationToken);

            if (record is null)
            {
                throw new InvalidOperationException(
                    $"No processing record found for event '{eventId}' and consumer '{consumerName}'.");
            }

            record.Status = Processed;
            record.ProcessedOnUtc = DateTime.UtcNow;
            record.FailedOnUtc = null;
            record.Error = null;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkAsFailedAsync(
            Guid eventId,
            string consumerName,
            string error,
            CancellationToken cancellationToken)
        {
            var record = await _dbContext.ProcessedIntegrationEvents
                .SingleOrDefaultAsync(
                    x => x.EventId == eventId && x.ConsumerName == consumerName,
                    cancellationToken);

            if (record is null)
            {
                throw new InvalidOperationException(
                    $"No processing record found for event '{eventId}' and consumer '{consumerName}'.");
            }

            record.Status = Failed;
            record.FailedOnUtc = DateTime.UtcNow;
            record.Error = error;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            // Aquí tienes que adaptar según proveedor:
            // SQL Server, PostgreSQL, etc.
            // Como base genérica, esta implementación es conservadora.
            return ex.InnerException is DbException;
        }
    }
}
