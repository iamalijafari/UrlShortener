using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UrlShortener.Application.IntegrationEvents;
using UrlShortener.Infrastructure.Messaging;
using UrlShortener.Infrastructure.Persistence;

namespace UrlShortener.Analytics.Worker.Workers;

public sealed class OutboxPublisherWorker : BackgroundService
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new(JsonSerializerDefaults.Web);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly OutboxOptions _options;
    private readonly ILogger<OutboxPublisherWorker> _logger;

    public OutboxPublisherWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<OutboxOptions> options,
        ILogger<OutboxPublisherWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(
            TimeSpan.FromMilliseconds(_options.PollingIntervalMilliseconds));

        do
        {
            try
            {
                await PublishBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to publish an outbox batch");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task PublishBatchAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var publisher = scope.ServiceProvider
            .GetRequiredService<IIntegrationEventPublisher>();

        await using var transaction = await dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        var messages = await dbContext.OutboxMessages
            .FromSqlRaw(
                """
                SELECT * FROM outbox_messages
                WHERE "PublishedAtUtc" IS NULL AND "Attempts" < 10
                ORDER BY "OccurredAtUtc"
                LIMIT {0}
                FOR UPDATE SKIP LOCKED
                """,
                _options.BatchSize)
            .ToArrayAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var integrationEvent =
                    JsonSerializer.Deserialize<UrlVisitedIntegrationEvent>(
                        message.Payload,
                        SerializerOptions)
                    ?? throw new JsonException("Outbox payload is empty.");

                await publisher.PublishAsync(integrationEvent, cancellationToken);
                message.PublishedAtUtc = DateTime.UtcNow;
                message.Error = null;
            }
            catch (Exception exception)
            {
                message.Attempts++;
                message.Error = exception.Message[..Math.Min(
                    exception.Message.Length,
                    2000)];
                _logger.LogWarning(
                    exception,
                    "Failed to publish outbox message {MessageId} on attempt {Attempt}",
                    message.Id,
                    message.Attempts);
                break;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if (messages.Length > 0)
        {
            _logger.LogInformation(
                "Processed {MessageCount} outbox messages",
                messages.Length);
        }
    }
}
