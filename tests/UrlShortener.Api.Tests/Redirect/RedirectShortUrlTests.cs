using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UrlShortener.Api.Tests.Common;
using UrlShortener.Application.Features.ShortUrls.Create;
using UrlShortener.Application.Features.ShortUrls.GetByCode;

namespace UrlShortener.Api.Tests.Redirect;

public sealed class RedirectShortUrlTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RedirectShortUrlTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateNoRedirectClient();
    }

    [Fact]
    public async Task Redirect_Should_Return_302()
    {
        var create = await _client.PostAsJsonAsync(
            "/api/shorturls",
            new { originalUrl = "https://google.com" });

        var created = await create.Content.ReadFromJsonAsync<CreateShortUrlResponse>();

        var response = await _client.GetAsync($"/{created!.ShortCode}");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Be("https://google.com/");
    }

    [Fact]
    public async Task Redirect_Should_Increment_ClickCount()
    {
        var create = await _client.PostAsJsonAsync(
            "/api/shorturls",
            new { originalUrl = "https://google.com" });

        var created = await create.Content.ReadFromJsonAsync<CreateShortUrlResponse>();

        await _client.GetAsync($"/{created!.ShortCode}");
        await _client.GetAsync($"/{created.ShortCode}");

        var stats = await _client.GetFromJsonAsync<GetShortUrlByCodeResponse>(
            $"/api/shorturls/{created.ShortCode}");

        stats!.ClickCount.Should().Be(2);
    }
}