namespace UrlShortener.Infrastructure.Messaging;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string ConnectionString { get; init; } = "amqp://guest:guest@localhost:5672/";
    public string Exchange { get; init; } = "urlshortener.events";
    public string Queue { get; init; } = "urlshortener.analytics";
    public string RoutingKey { get; init; } = "url.visited.v1";
}
