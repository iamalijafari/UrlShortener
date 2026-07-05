using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Tests.Common;

namespace UrlShortener.Api.Tests.Validation;

public sealed class CreateShortUrlValidationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CreateShortUrlValidationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_Should_Return_ProblemDetails_When_Url_Is_Empty()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/shorturls",
            new { originalUrl = "" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Validation Failed");
        problem.Status.Should().Be(400);
    }

    [Fact]
    public async Task Create_Should_Return_ProblemDetails_When_Url_Is_Invalid()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/shorturls",
            new { originalUrl = "hello" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Validation Failed");
        problem.Status.Should().Be(400);
    }
}