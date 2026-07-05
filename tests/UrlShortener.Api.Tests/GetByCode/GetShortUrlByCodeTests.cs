using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UrlShortener.Api.Tests.Common;
using UrlShortener.Application.Features.ShortUrls.Create;
using UrlShortener.Application.Features.ShortUrls.GetByCode;

namespace UrlShortener.Api.Tests.GetByCode;

public sealed class GetShortUrlByCodeTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public GetShortUrlByCodeTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetByCode_Should_Return_ShortUrl_Details()
    {
        // Arrange
        var createRequest = new
        {
            originalUrl = "https://google.com"
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/shorturls",
            createRequest);

        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content
            .ReadFromJsonAsync<CreateShortUrlResponse>();

        // Act
        var response = await _client.GetAsync(
            $"/api/shorturls/{created!.ShortCode}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<GetShortUrlByCodeResponse>();

        result.Should().NotBeNull();

        result!.ShortCode.Should().Be(created.ShortCode);
        result.OriginalUrl.Should().Be("https://google.com");
        result.ClickCount.Should().Be(0);
        result.IsActive.Should().BeTrue();

        result.CreatedAt.Should().BeCloseTo(
            DateTime.UtcNow,
            TimeSpan.FromSeconds(10));

        result.ExpiresAt.Should().NotBeNull();
    }
}