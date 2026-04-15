using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModularBackend.Application.Abstractions.Services.Messaging;
using ModularBackend.Infrastructure.Messaging.Outbox;
using ModularBackend.Infrastructure.Persistence;

namespace ModularBackend.Infrastructure.Messaging
{
    public sealed class PublisherWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PublisherWorker> _logger;

        public PublisherWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<PublisherWorker> logger)
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
                    var bus = scope.ServiceProvider.GetRequiredService<IMessagingBus>();

                    var pendingMessages = await dbContext.Set<OutboxMessage>()
                        .Where(x => x.ProcessedOnUtc == null && x.Attempts <= 5)
                        .OrderBy(x => x.OccurredOnUtc)
                        .Take(20)
                        .ToListAsync(stoppingToken);

                    foreach (var message in pendingMessages)
                    {
                        try
                        {
                            if(message.Attempts >= 5)
                            {
                                _logger.LogWarning("Outbox message {MessageId} has reached maximum retry attempts", message.Id);
                                continue;
                            }

                            var env = OutboxToIntegrationEventMapper.Map(message);

                            if (env != null)
                            {
                                await bus.PublishAsync(env, stoppingToken);

                                message.ProcessedOnUtc = DateTime.UtcNow;
                                message.LastAttemptOnUtc = DateTime.UtcNow;
                                message.Error = null;
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
