using UrlShortener.Domain.Common;

namespace UrlShortener.Domain.Entities;

public sealed class ShortUrl : BaseEntity
{
    public ShortUrl(
        string originalUrl,
        string shortCode,
        DateTime? expiresAt = null)
    {
        OriginalUrl = originalUrl;
        ShortCode = shortCode;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        IsActive = true;
    }

    public string OriginalUrl { get; private set; }
    public string ShortCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public int ClickCount { get; private set; }
    public bool IsActive { get; private set; }
}