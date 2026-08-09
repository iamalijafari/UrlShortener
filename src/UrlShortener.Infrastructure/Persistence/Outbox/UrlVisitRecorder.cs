using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions.Messaging;
using UrlShortener.Application.IntegrationEvents;
using UrlShortener.Infrastructure.Persistence.Models;

namespace UrlShortener.Infrastructure.Persistence.Outbox;

public sealed class UrlVisitRecorder : IUrlVisitRecorder
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new(JsonSerializerDefaults.Web);

    private readonly AppDbContext _dbContext;

    public UrlVisitRecorder(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> RecordAsync(
        UrlVisitedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        var updated = await _dbContext.ShortUrls
            .Where(shortUrl =>
                shortUrl.Id == integrationEvent.ShortUrlId &&
                shortUrl.IsActive &&
                (!shortUrl.ExpiresAt.HasValue ||
                    shortUrl.ExpiresAt > integrationEvent.VisitedAtUtc))
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    shortUrl => shortUrl.ClickCount,
                    shortUrl => shortUrl.ClickCount + 1),
                cancellationToken);

        if (updated == 0)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }

        _dbContext.OutboxMessages.Add(new OutboxMessage
        {
            Id = integrationEvent.EventId,
            Type = nameof(UrlVisitedIntegrationEvent),
            Payload = JsonSerializer.Serialize(integrationEvent, SerializerOptions),
            OccurredAtUtc = integrationEvent.VisitedAtUtc
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }
}
