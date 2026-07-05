using System.Net.Http.Json;
using FluentAssertions;
using UrlShortener.Api.Tests.Common;
using UrlShortener.Application.Features.ShortUrls.Create;
using UrlShortener.Application.Features.ShortUrls.GetByCode;

namespace UrlShortener.Api.Tests.Contract;

public sealed class ContractTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ContractTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetByCode_Should_Return_Correct_Structure()
    {
        var create = await _client.PostAsJsonAsync(
            "/api/shorturls",
            new { originalUrl = "https://google.com" });

        var created = await create.Content.ReadFromJsonAsync<CreateShortUrlResponse>();

        var result = await _client.GetFromJsonAsync<GetShortUrlByCodeResponse>(
            $"/api/shorturls/{created!.ShortCode}");

        result.Should().NotBeNull();
        result!.ShortCode.Should().Be(created.ShortCode);
        result.OriginalUrl.Should().Be("https://google.com");
    }
}