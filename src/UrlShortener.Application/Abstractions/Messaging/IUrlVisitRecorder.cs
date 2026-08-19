using UrlShortener.Application.IntegrationEvents;

namespace UrlShortener.Application.Abstractions.Messaging;

public interface IUrlVisitRecorder
{
    Task<bool> RecordAsync(
        UrlVisitedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken);
}
