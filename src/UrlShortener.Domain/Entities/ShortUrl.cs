using UrlShortener.Domain.Common;
using UrlShortener.Domain.Exceptions;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Domain.Entities;

public sealed class ShortUrl : BaseEntity
{
    private ShortUrl()
    { }

    public ShortUrl(
        OriginalUrl originalUrl,
        ShortCode shortCode,
        DateTime createdAt,
        DateTime? expiresAt = null)
    {
        OriginalUrl = originalUrl;
        ShortCode = shortCode;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        ClickCount = 0;
        IsActive = true;
    }

    public OriginalUrl OriginalUrl { get; private set; } = null!;
    public ShortCode ShortCode { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public int ClickCount { get; private set; }
    public bool IsActive { get; private set; }

    public bool IsExpired(DateTime utcNow)
    {
        return ExpiresAt.HasValue &&
            ExpiresAt.Value <= utcNow;
    }

    public bool CanRedirect(DateTime utcNow)
    {
        return IsActive && !IsExpired(utcNow);
    }

    public void RegisterClick(DateTime utcNow)
    {
        if (!CanRedirect(utcNow))
            throw new DomainException("Short URL cannot be used.");

        ClickCount++;
    }

    public void Disable()
    {
        IsActive = false;
    }
}