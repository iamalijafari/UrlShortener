using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UrlShortener.Api.Tests.Common;
using UrlShortener.Application.Features.ShortUrls.Create;

namespace UrlShortener.Api.Tests.Expiration;

[Collection(IntegrationTestCollection.Name)]
public sealed class ExpirationTests
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
                expiresAt = DateTime.UtcNow.AddSeconds(1)
            });

        var created = await create.Content.ReadFromJsonAsync<CreateShortUrlResponse>();
        await Task.Delay(TimeSpan.FromSeconds(2));

        var response = await _client.GetAsync($"/{created!.ShortCode}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
