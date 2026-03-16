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
        private readonly INotificationPublisher _notificationPublisher;
        private IDbContextTransaction? _transaction;
        private List<IDomainEvent> _committedDomainEvents = new();

        public bool HasActiveTransaction => _transaction is not null;

        public UnitOfWork(
            ApplicationDbContext dbContext,
            INotificationPublisher notificationPublisher)
        {
            _dbContext = dbContext;
            _notificationPublisher = notificationPublisher;
        }

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction is not null) return;
            _transaction = await _dbContext.Database.BeginTransactionAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            var domainEvents = _dbContext.CollectDomainEvents(deleteDomainsEvents: true).ToList();

            var outboxMessages = OutboxMessageFactory.Create(domainEvents);

            if (outboxMessages.Count > 0)
                await _dbContext.OutboxMessages.AddRangeAsync(outboxMessages, ct);

            await _dbContext.SaveChangesAsync(ct);

            _committedDomainEvents.AddRange(domainEvents);
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            if (_transaction is null)
                throw new InvalidOperationException("No active transaction.");

            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;

            foreach (var domainEvent in _committedDomainEvents)
            {
                var notification = EventsMaps.Map(domainEvent);
                if (notification is not null)
                {
                    await _notificationPublisher.Publish(notification, ct);
                }
            }

            _committedDomainEvents.Clear();
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_transaction is null) return;

            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
            _committedDomainEvents.Clear();
        }
    }
}
