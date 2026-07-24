namespace UrlShortener.Application.IntegrationEvents;

public sealed record UrlVisitedIntegrationEvent(
    Guid EventId,
    Guid ShortUrlId,
    string ShortCode,
    DateTime VisitedAtUtc);
