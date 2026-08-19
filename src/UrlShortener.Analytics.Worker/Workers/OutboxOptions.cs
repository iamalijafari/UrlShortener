namespace UrlShortener.Analytics.Worker.Workers;

public sealed class OutboxOptions
{
    public const string SectionName = "Outbox";

    public int BatchSize { get; init; } = 50;
    public int PollingIntervalMilliseconds { get; init; } = 1000;
}
