namespace UrlShortener.Infrastructure.Caching;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; init; } = "localhost:6379";
    public int RedirectTtlMinutes { get; init; } = 60;
}
