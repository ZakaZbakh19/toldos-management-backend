using Microsoft.EntityFrameworkCore.Storage;
using ModularBackend.Application.Abstractions.Events;
using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Application.Products.Events;
using ModularBackend.Domain.Abstractions;
using ModularBackend.Domain.Events;
using ModularBackend.Infrastructure.Outbox;
using ModularBackend.Infrastructure.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Persistance
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction? _currentTransaction;
        private readonly INotificationPublisher _publisher;

        public UnitOfWork(ApplicationDbContext dbContext,
            INotificationPublisher notificationPublisher)
        {
            _dbContext = dbContext;
            _publisher = notificationPublisher;
        }

        public bool HasActiveTransaction => _currentTransaction is not null;

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is not null)
                return;

            _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEvents = _dbContext.CollectDomainEvents();

            var outboxMessages = OutboxMessageFactory.Create(domainEvents);

            if (outboxMessages.Count > 0)
            {
                await _dbContext.Set<OutboxMessage>()
                    .AddRangeAsync(outboxMessages, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is null)
                throw new InvalidOperationException("No active transaction.");

            var events = _dbContext.CollectDomainEvents(deleteDomainsEvents: true);

            await _currentTransaction.CommitAsync(cancellationToken);

            foreach (var domainEvent in events)
            {
                var notification = EventsMaps.Map(domainEvent);
                await _publisher.Publish(notification, cancellationToken);
            }

            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is null)
                return;

            await _currentTransaction.RollbackAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }
}
