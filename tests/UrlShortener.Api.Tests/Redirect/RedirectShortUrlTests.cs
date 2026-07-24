using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UrlShortener.Api.Tests.Common;
using UrlShortener.Application.Features.ShortUrls.Create;
using UrlShortener.Application.Features.ShortUrls.GetByCode;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Application.IntegrationEvents;
using UrlShortener.Infrastructure.Caching;
using UrlShortener.Infrastructure.Persistence;
using UrlShortener.Infrastructure.Persistence.Analytics;
using StackExchange.Redis;

namespace UrlShortener.Api.Tests.Redirect;

[Collection(IntegrationTestCollection.Name)]
public sealed class RedirectShortUrlTests
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public RedirectShortUrlTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
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

        await ProcessOutboxEventsAsync(created.ShortCode);

        var stats = await _client.GetFromJsonAsync<GetShortUrlByCodeResponse>(
            $"/api/shorturls/{created.ShortCode}");

        stats!.ClickCount.Should().Be(2);
    }

    [Fact]
    public async Task Redirect_Should_Cache_Lookup_In_Redis()
    {
        var create = await _client.PostAsJsonAsync(
            "/api/shorturls",
            new { originalUrl = "https://example.com" });
        var created = await create.Content
            .ReadFromJsonAsync<CreateShortUrlResponse>();

        await _client.GetAsync($"/{created!.ShortCode}");

        using var redis = await ConnectionMultiplexer.ConnectAsync(
            _factory.RedisConnectionString);
        var cached = await redis.GetDatabase().StringGetAsync(
            RedisRedirectCache.Key(created.ShortCode));

        cached.HasValue.Should().BeTrue();
    }

    private async Task ProcessOutboxEventsAsync(string shortCode)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var processor = scope.ServiceProvider
            .GetRequiredService<IUrlVisitedEventProcessor>();

        var messages = dbContext.OutboxMessages
            .AsEnumerable()
            .Where(x => x.Payload.Contains(shortCode, StringComparison.Ordinal))
            .ToArray();

        messages.Should().HaveCount(2);

        foreach (var message in messages)
        {
            var integrationEvent =
                JsonSerializer.Deserialize<UrlVisitedIntegrationEvent>(
                    message.Payload,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

            integrationEvent.Should().NotBeNull();
            (await processor.ProcessAsync(
                integrationEvent!,
                CancellationToken.None)).Should().BeTrue();
        }

        var duplicate = JsonSerializer.Deserialize<UrlVisitedIntegrationEvent>(
            messages[0].Payload,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        (await processor.ProcessAsync(
            duplicate!,
            CancellationToken.None)).Should().BeFalse();
    }
}
