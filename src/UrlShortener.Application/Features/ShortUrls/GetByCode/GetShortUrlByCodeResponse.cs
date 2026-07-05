namespace UrlShortener.Application.Features.ShortUrls.GetByCode;

public sealed record GetShortUrlByCodeResponse(
    string ShortCode,
    string OriginalUrl,
    DateTime CreatedAt,
    DateTime? ExpiresAt,
    int ClickCount,
    bool IsActive);