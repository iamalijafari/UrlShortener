namespace UrlShortener.Application.Abstractions.Services;

public interface IRedirectCache
{
    Task<RedirectCacheEntry?> GetAsync(
        string shortCode,
        CancellationToken cancellationToken);

    Task SetAsync(
        string shortCode,
        RedirectCacheEntry entry,
        CancellationToken cancellationToken);

    Task RemoveAsync(
        string shortCode,
        CancellationToken cancellationToken);
}

public sealed record RedirectCacheEntry(
    Guid ShortUrlId,
    string OriginalUrl,
    DateTime? ExpiresAtUtc);
