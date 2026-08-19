using Microsoft.Extensions.Diagnostics.HealthChecks;
using UrlShortener.Infrastructure.Messaging;

namespace UrlShortener.Infrastructure.Health;

public sealed class RabbitMqHealthCheck : IHealthCheck
{
    private readonly IRabbitMqConnection _connection;

    public RabbitMqHealthCheck(IRabbitMqConnection connection)
    {
        _connection = connection;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return Task.FromResult(
                _connection.GetConnection().IsOpen
                    ? HealthCheckResult.Healthy("RabbitMQ connection is open.")
                    : HealthCheckResult.Unhealthy("RabbitMQ connection is closed."));
        }
        catch (Exception exception)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    "RabbitMQ is unavailable.",
                    exception));
        }
    }
}
