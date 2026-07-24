namespace UrlShortener.Infrastructure.Persistence.Models;

public sealed class DailyVisitStatistic
{
    public Guid ShortUrlId { get; set; }
    public DateOnly Date { get; set; }
    public long ClickCount { get; set; }
    public DateTime LastVisitedAtUtc { get; set; }
}
