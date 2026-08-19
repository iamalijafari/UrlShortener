using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UrlShortener.Application.Common.Observability;
using UrlShortener.Application.IntegrationEvents;

namespace UrlShortener.Infrastructure.Messaging;

public sealed class RabbitMqIntegrationEventPublisher : IIntegrationEventPublisher
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new(JsonSerializerDefaults.Web);

    private readonly IRabbitMqConnection _connection;
    private readonly RabbitMqOptions _options;

    public RabbitMqIntegrationEventPublisher(
        IRabbitMqConnection connection,
        IOptions<RabbitMqOptions> options)
    {
        _connection = connection;
        _options = options.Value;
    }

    public Task PublishAsync(
        UrlVisitedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var activity = Telemetry.ActivitySource.StartActivity(
            "rabbitmq.url-visited.publish");
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination", _options.Exchange);
        activity?.SetTag("messaging.message_id", integrationEvent.EventId);

        using var channel = _connection.GetConnection().CreateModel();
        channel.ExchangeDeclare(
            _options.Exchange,
            ExchangeType.Direct,
            durable: true,
            autoDelete: false);
        channel.QueueDeclare(
            _options.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false);
        channel.QueueBind(
            _options.Queue,
            _options.Exchange,
            _options.RoutingKey);
        channel.ConfirmSelect();

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.Type = nameof(UrlVisitedIntegrationEvent);
        properties.MessageId = integrationEvent.EventId.ToString();

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(integrationEvent, SerializerOptions));

        channel.BasicPublish(
            _options.Exchange,
            _options.RoutingKey,
            mandatory: true,
            properties,
            body);
        channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(10));

        return Task.CompletedTask;
    }
}
