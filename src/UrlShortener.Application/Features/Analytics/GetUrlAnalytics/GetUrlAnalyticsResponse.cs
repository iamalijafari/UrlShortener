namespace UrlShortener.Application.Features.Analytics.GetUrlAnalytics;

public sealed record GetUrlAnalyticsResponse(
    string ShortCode,
    DateOnly From,
    DateOnly To,
    long TotalClicks,
    IReadOnlyCollection<DailyUrlAnalytics> Daily);

public sealed record DailyUrlAnalytics(
    DateOnly Date,
    long ClickCount);
