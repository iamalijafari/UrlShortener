using UrlShortener.Application.IntegrationEvents;

namespace UrlShortener.Infrastructure.Messaging;

public interface IIntegrationEventPublisher
{
    Task PublishAsync(
        UrlVisitedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken);
}
