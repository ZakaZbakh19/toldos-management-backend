using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly int MaxRetryAttempts = 0;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PublisherWorker> _logger;

        public PublisherWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<PublisherWorker> logger,
            IConfiguration conf)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            MaxRetryAttempts = conf.GetValue<int>("Security:MaxRetryAttempts", 5);
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
                        .Where(x => x.ProcessedOnUtc == null && x.Attempts < MaxRetryAttempts)
                        .OrderBy(x => x.OccurredOnUtc)
                        .Take(20)
                        .ToListAsync(stoppingToken);

                    foreach (var message in pendingMessages)
                    {
                        try
                        {
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

                            if (message.Attempts >= MaxRetryAttempts)
                            {
                                _logger.LogError(ex,
                                    "Outbox message {MessageId} has reached max retry attempts and will be marked as failed",
                                    message.Id);
                            }

                            message.LastAttemptOnUtc = DateTime.UtcNow;
                            message.Error = ex.Message;

                            _logger.LogError(
                                ex,
                                "Error publishing outbox message {MessageId} of type {EventType} on attempt {Attempt}",
                                message.Id,
                                message.Type,
                                message.Attempts);
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
