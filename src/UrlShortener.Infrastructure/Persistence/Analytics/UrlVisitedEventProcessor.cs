using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Common.Observability;
using UrlShortener.Application.IntegrationEvents;

namespace UrlShortener.Infrastructure.Persistence.Analytics;

public sealed class UrlVisitedEventProcessor : IUrlVisitedEventProcessor
{
    private readonly AppDbContext _dbContext;

    public UrlVisitedEventProcessor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ProcessAsync(
        UrlVisitedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        using var activity = Telemetry.ActivitySource.StartActivity(
            "analytics.url-visited.process");
        activity?.SetTag("messaging.message_id", integrationEvent.EventId);
        activity?.SetTag("url.short_code", integrationEvent.ShortCode);

        await using var transaction = await _dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        var inserted = await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
            INSERT INTO processed_events ("Id", "ProcessedAtUtc")
            VALUES ({integrationEvent.EventId}, {DateTime.UtcNow})
            ON CONFLICT ("Id") DO NOTHING
            """,
            cancellationToken);

        if (inserted == 0)
        {
            await transaction.CommitAsync(cancellationToken);
            activity?.SetTag("messaging.duplicate", true);
            return false;
        }

        var visitDate = DateOnly.FromDateTime(integrationEvent.VisitedAtUtc);
        await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
            INSERT INTO daily_visit_statistics
                ("ShortUrlId", "Date", "ClickCount", "LastVisitedAtUtc")
            VALUES
                ({integrationEvent.ShortUrlId}, {visitDate}, 1, {integrationEvent.VisitedAtUtc})
            ON CONFLICT ("ShortUrlId", "Date")
            DO UPDATE SET
                "ClickCount" = daily_visit_statistics."ClickCount" + 1,
                "LastVisitedAtUtc" = EXCLUDED."LastVisitedAtUtc"
            """,
            cancellationToken);

        await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
            UPDATE short_urls
            SET "ClickCount" = "ClickCount" + 1
            WHERE "Id" = {integrationEvent.ShortUrlId}
            """,
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return true;
    }
}
