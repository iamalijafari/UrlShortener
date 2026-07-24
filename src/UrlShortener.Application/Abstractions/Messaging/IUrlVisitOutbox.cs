using UrlShortener.Application.IntegrationEvents;

namespace UrlShortener.Application.Abstractions.Messaging;

public interface IUrlVisitOutbox
{
    void Add(UrlVisitedIntegrationEvent integrationEvent);
}
