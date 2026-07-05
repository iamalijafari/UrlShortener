using System.Net;
using FluentAssertions;
using UrlShortener.Api.Tests.Common;

namespace UrlShortener.Api.Tests.Create;

public sealed class HealthTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Application_Should_Start()
    {
        // Act
        var response = await _client.GetAsync("/swagger/index.html");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}