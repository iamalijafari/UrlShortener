using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UrlShortener.Api.Tests.Common;
using UrlShortener.Application.Features.ShortUrls.Create;

namespace UrlShortener.Api.Tests.Expiration;

public sealed class ExpirationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ExpirationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Redirect_Should_Return_404_When_Expired()
    {
        var create = await _client.PostAsJsonAsync(
            "/api/shorturls",
            new
            {
                originalUrl = "https://google.com",
                expiresAt = DateTime.UtcNow.AddSeconds(-10)
            });

        var created = await create.Content.ReadFromJsonAsync<CreateShortUrlResponse>();

        var response = await _client.GetAsync($"/{created!.ShortCode}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}