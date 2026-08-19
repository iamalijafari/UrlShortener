namespace UrlShortener.Application.Abstractions.Services;

public interface IUrlAnalyticsReader
{
    Task<IReadOnlyCollection<DailyVisitCount>> GetDailyVisitsAsync(
        Guid shortUrlId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken);
}

public sealed record DailyVisitCount(
    DateOnly Date,
    long ClickCount);
