using FluentAssertions;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Exceptions;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Domain.Tests.Entities;

public class ShortUrlTests
{
    [Fact]
    public void RegisterClick_Should_Increment_ClickCount()
    {
        var now = TestNow;

        var shortUrl = new ShortUrl(
            OriginalUrl.Create("https://google.com"),
            ShortCode.Create("abc123"),
            now);

        shortUrl.RegisterClick(now);

        shortUrl.ClickCount.Should().Be(1);
    }

    [Fact]
    public void Disable_Should_Deactivate_ShortUrl()
    {
        var now = TestNow;

        var shortUrl = new ShortUrl(
            OriginalUrl.Create("https://google.com"),
            ShortCode.Create("abc123"),
            now);

        shortUrl.Disable();

        shortUrl.IsActive.Should().BeFalse();
    }

    [Fact]
    public void RegisterClick_Should_Throw_When_Expired()
    {
        var createdAt = TestNow;
        var expiresAt = createdAt.AddDays(1);

        var shortUrl = new ShortUrl(
            OriginalUrl.Create("https://google.com"),
            ShortCode.Create("abc123"),
            createdAt,
            expiresAt);

        var now = createdAt.AddDays(2);

        var action = () => shortUrl.RegisterClick(now);

        action.Should().Throw<DomainException>();
    }

    private static readonly DateTime TestNow = new(2026, 7, 5, 10, 0, 0, DateTimeKind.Utc);
}