using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace UrlShortener.Api.Tests.Common;

public sealed class InfrastructureFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder()
            .WithImage("postgres:17")
            .WithDatabase("urlshortener_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private readonly RedisContainer _redis =
        new RedisBuilder()
            .WithImage("redis:7.4-alpine")
            .Build();

    private readonly RabbitMqContainer _rabbitMq =
        new RabbitMqBuilder()
            .WithImage("rabbitmq:4-management-alpine")
            .Build();

    public string PostgreSqlConnectionString => _postgres.GetConnectionString();
    public string RedisConnectionString => _redis.GetConnectionString();
    public string RabbitMqConnectionString => _rabbitMq.GetConnectionString();

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            _postgres.StartAsync(),
            _redis.StartAsync(),
            _rabbitMq.StartAsync());
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            _postgres.DisposeAsync().AsTask(),
            _redis.DisposeAsync().AsTask(),
            _rabbitMq.DisposeAsync().AsTask());
    }
}
