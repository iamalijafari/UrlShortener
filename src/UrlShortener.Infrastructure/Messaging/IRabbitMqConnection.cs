using RabbitMQ.Client;

namespace UrlShortener.Infrastructure.Messaging;

public interface IRabbitMqConnection
{
    IConnection GetConnection();
}
