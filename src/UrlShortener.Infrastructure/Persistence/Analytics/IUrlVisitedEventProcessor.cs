using UrlShortener.Application.IntegrationEvents;

namespace UrlShortener.Infrastructure.Persistence.Analytics;

public interface IUrlVisitedEventProcessor
{
    Task<bool> ProcessAsync(
        UrlVisitedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken);
}
