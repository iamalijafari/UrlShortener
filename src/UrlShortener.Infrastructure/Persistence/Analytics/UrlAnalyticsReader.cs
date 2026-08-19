using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions.Services;

namespace UrlShortener.Infrastructure.Persistence.Analytics;

public sealed class UrlAnalyticsReader : IUrlAnalyticsReader
{
    private readonly AppDbContext _dbContext;

    public UrlAnalyticsReader(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<DailyVisitCount>> GetDailyVisitsAsync(
        Guid shortUrlId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken)
    {
        return await _dbContext.DailyVisitStatistics
            .AsNoTracking()
            .Where(x =>
                x.ShortUrlId == shortUrlId &&
                x.Date >= from &&
                x.Date <= to)
            .OrderBy(x => x.Date)
            .Select(x => new DailyVisitCount(x.Date, x.ClickCount))
            .ToArrayAsync(cancellationToken);
    }
}
