using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UrlShortener.Api.Tests.Common;
using UrlShortener.Application.IntegrationEvents;
using UrlShortener.Infrastructure.Messaging;

namespace UrlShortener.Api.Tests.Infrastructure;

[Collection(IntegrationTestCollection.Name)]
public sealed class RabbitMqIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;

    public RabbitMqIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Publisher_Should_Send_UrlVisited_Event_To_RabbitMq()
    {
        using var scope = _factory.Services.CreateScope();
        var publisher = scope.ServiceProvider
            .GetRequiredService<IIntegrationEventPublisher>();
        var options = scope.ServiceProvider
            .GetRequiredService<IOptions<RabbitMqOptions>>()
            .Value;

        var connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(_factory.RabbitMqConnectionString)
        };
        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(
            options.Exchange,
            ExchangeType.Direct,
            durable: true,
            autoDelete: false);
        var queue = channel.QueueDeclare(
            queue: string.Empty,
            durable: false,
            exclusive: true,
            autoDelete: true);
        channel.QueueBind(
            queue.QueueName,
            options.Exchange,
            options.RoutingKey);

        var integrationEvent = new UrlVisitedIntegrationEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "abc123",
            DateTime.UtcNow);

        await publisher.PublishAsync(
            integrationEvent,
            CancellationToken.None);

        var delivery = channel.BasicGet(queue.QueueName, autoAck: true);
        delivery.Should().NotBeNull();

        var received = JsonSerializer.Deserialize<UrlVisitedIntegrationEvent>(
            delivery!.Body.Span,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        received.Should().Be(integrationEvent);
    }
}
