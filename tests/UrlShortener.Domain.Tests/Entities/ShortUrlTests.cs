using FluentAssertions;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Domain.Tests.Entities;

public class ShortUrlTests
{
    [Fact]
    public void RegisterClick_Should_Increment_ClickCount()
    {
        var shortUrl = new ShortUrl(
            OriginalUrl.Create("https://google.com"),
            ShortCode.Create("abc123"));

        shortUrl.RegisterClick();

        shortUrl.ClickCount.Should().Be(1);
    }

    [Fact]
    public void Disable_Should_Deactivate_ShortUrl()
    {
        var shortUrl = new ShortUrl(
            OriginalUrl.Create("https://google.com"),
            ShortCode.Create("abc123"));

        shortUrl.Disable();

        shortUrl.IsActive.Should().BeFalse();
    }
}