using System.Net;
using FluentAssertions;
using UrlShortener.Api.Tests.Common;

namespace UrlShortener.Api.Tests.NotFound;

public sealed class NotFoundTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public NotFoundTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetByCode_Should_Return_404_When_NotFound()
    {
        var response = await _client.GetAsync("/api/shorturls/unknown");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Redirect_Should_Return_404_When_NotFound()
    {
        var response = await _client.GetAsync("/unknown");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}