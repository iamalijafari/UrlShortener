using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UrlShortener.Api.Tests.Common;
using UrlShortener.Application.Features.ShortUrls.Create;

namespace UrlShortener.Api.Tests.Create;

public sealed class CreateShortUrlTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CreateShortUrlTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_Should_Return_400_When_Url_Is_Empty()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/shorturls",
            new { originalUrl = "" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_Should_Return_400_When_Url_Is_Invalid()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/shorturls",
            new { originalUrl = "hello" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_Should_Return_201_And_Location_Header()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/shorturls",
            new { originalUrl = "https://google.com" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_Should_Generate_Unique_ShortCodes()
    {
        var r1 = await _client.PostAsJsonAsync("/api/shorturls", new { originalUrl = "https://google.com" });
        var r2 = await _client.PostAsJsonAsync("/api/shorturls", new { originalUrl = "https://google.com" });

        var c1 = await r1.Content.ReadFromJsonAsync<CreateShortUrlResponse>();
        var c2 = await r2.Content.ReadFromJsonAsync<CreateShortUrlResponse>();

        c1!.ShortCode.Should().NotBe(c2!.ShortCode);
    }
}