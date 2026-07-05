using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UrlShortener.Api.Tests.Common;
using UrlShortener.Application.Features.ShortUrls.Create;

namespace UrlShortener.Api.Tests.Disable;

public sealed class DisableTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DisableTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Redirect_Should_Return_404_When_Disabled()
    {
        var create = await _client.PostAsJsonAsync(
            "/api/shorturls",
            new { originalUrl = "https://google.com" });

        var created = await create.Content.ReadFromJsonAsync<CreateShortUrlResponse>();

        await _client.PostAsync($"/api/shorturls/{created!.ShortCode}/disable", null);

        var response = await _client.GetAsync($"/{created.ShortCode}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}