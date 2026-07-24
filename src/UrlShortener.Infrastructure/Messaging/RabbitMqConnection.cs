using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace UrlShortener.Infrastructure.Messaging;

public sealed class RabbitMqConnection : IRabbitMqConnection, IDisposable
{
    private readonly object _sync = new();
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;

    public RabbitMqConnection(IOptions<RabbitMqOptions> options)
    {
        _factory = new ConnectionFactory
        {
            Uri = new Uri(options.Value.ConnectionString),
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true
        };
    }

    public IConnection GetConnection()
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        lock (_sync)
        {
            if (_connection is not { IsOpen: true })
            {
                _connection?.Dispose();
                _connection = _factory.CreateConnection("urlshortener");
            }

            return _connection;
        }
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
