namespace UrlShortener.Infrastructure.Persistence.Models;

public sealed class ProcessedEvent
{
    public Guid Id { get; set; }
    public DateTime ProcessedAtUtc { get; set; }
}
