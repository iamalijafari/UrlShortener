using System.Net;
using FluentAssertions;
using UrlShortener.Api.Tests.Common;

namespace UrlShortener.Api.Tests.Create;

[Collection(IntegrationTestCollection.Name)]
public sealed class HealthTests
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
        var response = await _client.GetAsync("/health/ready");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
