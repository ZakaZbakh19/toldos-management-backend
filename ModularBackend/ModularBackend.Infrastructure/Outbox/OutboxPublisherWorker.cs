using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModularBackend.Application.IntegrationEvents;
using ModularBackend.Infrastructure.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ModularBackend.Infrastructure.Outbox
{
    public sealed class OutboxPublisherWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxPublisherWorker> _logger;

        public OutboxPublisherWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<OutboxPublisherWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var bus = scope.ServiceProvider.GetRequiredService<IIntegrationEventBus>();

                    var pendingMessages = await dbContext.Set<OutboxMessage>()
                        .Where(x => x.ProcessedOnUtc == null)
                        .OrderBy(x => x.OccurredOnUtc)
                        .Take(20)
                        .ToListAsync(stoppingToken);

                    foreach (var message in pendingMessages)
                    {
                        try
                        {
                            var env = OutboxEventMap.Map(message);

                            if (env != null)
                            {
                                await bus.PublishAsync(env, stoppingToken);
                                message.ProcessedOnUtc = DateTime.UtcNow;
                                message.Error = null;
                            }
                            else
                            {
                                throw new InvalidOperationException($"Unknown integration event type: {message.Type}");
                            }
                        }
                        catch (Exception ex)
                        {
                            message.Attempts++;
                            message.LastAttemptOnUtc = DateTime.UtcNow;
                            message.Error = ex.Message;
                            _logger.LogError(ex, "Error publishing outbox message {MessageId}", message.Id);
                        }
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Outbox worker failed");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
