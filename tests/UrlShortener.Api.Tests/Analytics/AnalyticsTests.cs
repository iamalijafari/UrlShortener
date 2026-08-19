using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UrlShortener.Api.Tests.Common;
using UrlShortener.Application.Features.Analytics.GetUrlAnalytics;

namespace UrlShortener.Api.Tests.Analytics;

[Collection(IntegrationTestCollection.Name)]
public sealed class AnalyticsTests
{
    private readonly HttpClient _client;

    public AnalyticsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAnalytics_Should_Return_Zero_Filled_Daily_Series()
    {
        var create = await _client.PostAsJsonAsync(
            "/api/shorturls",
            new { originalUrl = "https://example.com/analytics" });
        var created = await create.Content
            .ReadFromJsonAsync<UrlShortener.Application.Features.ShortUrls.Create.CreateShortUrlResponse>();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var response = await _client.GetAsync(
            $"/api/shorturls/{created!.ShortCode}/analytics?from={today:yyyy-MM-dd}&to={today:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var analytics = await response.Content
            .ReadFromJsonAsync<GetUrlAnalyticsResponse>();

        analytics.Should().NotBeNull();
        analytics!.TotalClicks.Should().Be(0);
        analytics.Daily.Should().ContainSingle();
        analytics.Daily.Single().Date.Should().Be(today);
    }
}
