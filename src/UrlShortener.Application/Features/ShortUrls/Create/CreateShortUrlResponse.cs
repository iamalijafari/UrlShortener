namespace UrlShortener.Application.Features.ShortUrls.Create;

public sealed record CreateShortUrlResponse(
    string ShortCode,
    string ShortUrl
);