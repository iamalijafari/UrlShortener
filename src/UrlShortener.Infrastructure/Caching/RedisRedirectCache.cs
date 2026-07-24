using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using UrlShortener.Application.Abstractions.Services;
using UrlShortener.Application.Common.Observability;

namespace UrlShortener.Infrastructure.Caching;

public sealed class RedisRedirectCache : IRedirectCache
{
    private const string KeyPrefix = "redirect:v1:";
    private static readonly JsonSerializerOptions SerializerOptions =
        new(JsonSerializerDefaults.Web);

    private readonly IDatabase _database;
    private readonly RedisOptions _options;
    private readonly ILogger<RedisRedirectCache> _logger;

    public RedisRedirectCache(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisOptions> options,
        ILogger<RedisRedirectCache> logger)
    {
        _database = connectionMultiplexer.GetDatabase();
        _options = options.Value;
        _logger = logger;
    }

    public async Task<RedirectCacheEntry?> GetAsync(
        string shortCode,
        CancellationToken cancellationToken)
    {
        using var activity = Telemetry.ActivitySource.StartActivity("redis.redirect.get");
        try
        {
            var value = await _database.StringGetAsync(Key(shortCode));

            return value.HasValue
                ? JsonSerializer.Deserialize<RedirectCacheEntry>(
                    value.ToString(),
                    SerializerOptions)
                : null;
        }
        catch (RedisException exception)
        {
            _logger.LogWarning(
                exception,
                "Redis lookup failed for short code {ShortCode}; using PostgreSQL",
                shortCode);
            return null;
        }
    }

    public async Task SetAsync(
        string shortCode,
        RedirectCacheEntry entry,
        CancellationToken cancellationToken)
    {
        using var activity = Telemetry.ActivitySource.StartActivity("redis.redirect.set");

        var ttl = TimeSpan.FromMinutes(_options.RedirectTtlMinutes);
        if (entry.ExpiresAtUtc.HasValue)
        {
            var untilExpiration = entry.ExpiresAtUtc.Value - DateTime.UtcNow;
            if (untilExpiration <= TimeSpan.Zero)
            {
                return;
            }

            ttl = untilExpiration < ttl ? untilExpiration : ttl;
        }

        try
        {
            var value = JsonSerializer.Serialize(entry, SerializerOptions);
            await _database.StringSetAsync(Key(shortCode), value, ttl);
        }
        catch (RedisException exception)
        {
            _logger.LogWarning(
                exception,
                "Redis cache fill failed for short code {ShortCode}",
                shortCode);
        }
    }

    public async Task RemoveAsync(
        string shortCode,
        CancellationToken cancellationToken)
    {
        using var activity = Telemetry.ActivitySource.StartActivity("redis.redirect.remove");
        try
        {
            await _database.KeyDeleteAsync(Key(shortCode));
        }
        catch (RedisException exception)
        {
            _logger.LogWarning(
                exception,
                "Redis cache eviction failed for short code {ShortCode}",
                shortCode);
        }
    }

    public static string Key(string shortCode) =>
        $"{KeyPrefix}{shortCode.ToLowerInvariant()}";
}
