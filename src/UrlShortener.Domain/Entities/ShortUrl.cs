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
        DateTime? expiresAt = null)
    {
        OriginalUrl = originalUrl;
        ShortCode = shortCode;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        ClickCount = 0;
        IsActive = true;
    }

    public OriginalUrl OriginalUrl { get; private set; }
    public ShortCode ShortCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public int ClickCount { get; private set; }
    public bool IsActive { get; private set; }

    public bool IsExpired()
    {
        return ExpiresAt.HasValue &&
               ExpiresAt.Value <= DateTime.UtcNow;
    }

    public bool CanRedirect()
    {
        return IsActive && !IsExpired();
    }

    public void RegisterClick()
    {
        if (!CanRedirect())
            throw new DomainException("Short URL cannot be used.");

        ClickCount++;
    }

    public void Disable()
    {
        IsActive = false;
    }
}