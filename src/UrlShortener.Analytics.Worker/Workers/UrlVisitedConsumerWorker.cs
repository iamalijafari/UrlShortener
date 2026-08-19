using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UrlShortener.Application.IntegrationEvents;
using UrlShortener.Infrastructure.Messaging;
using UrlShortener.Infrastructure.Persistence.Analytics;

namespace UrlShortener.Analytics.Worker.Workers;

public sealed class UrlVisitedConsumerWorker : BackgroundService
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new(JsonSerializerDefaults.Web);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRabbitMqConnection _connection;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<UrlVisitedConsumerWorker> _logger;
    private IModel? _channel;

    public UrlVisitedConsumerWorker(
        IServiceScopeFactory scopeFactory,
        IRabbitMqConnection connection,
        IOptions<RabbitMqOptions> options,
        ILogger<UrlVisitedConsumerWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _connection = connection;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = _connection.GetConnection().CreateModel();
        _channel.ExchangeDeclare(
            _options.Exchange,
            ExchangeType.Direct,
            durable: true,
            autoDelete: false);
        _channel.QueueDeclare(
            _options.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false);
        _channel.QueueBind(
            _options.Queue,
            _options.Exchange,
            _options.RoutingKey);
        _channel.BasicQos(0, prefetchCount: 16, global: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += OnMessageReceivedAsync;

        _channel.BasicConsume(
            _options.Queue,
            autoAck: false,
            consumer);

        _logger.LogInformation(
            "Consuming URL visit events from {Queue}",
            _options.Queue);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Expected during a graceful shutdown.
        }
    }

    private async Task OnMessageReceivedAsync(
        object sender,
        BasicDeliverEventArgs eventArgs)
    {
        if (_channel is null)
        {
            return;
        }

        try
        {
            var payload = Encoding.UTF8.GetString(eventArgs.Body.Span);
            var integrationEvent =
                JsonSerializer.Deserialize<UrlVisitedIntegrationEvent>(
                    payload,
                    SerializerOptions)
                ?? throw new JsonException("RabbitMQ payload is empty.");

            await using var scope = _scopeFactory.CreateAsyncScope();
            var processor = scope.ServiceProvider
                .GetRequiredService<IUrlVisitedEventProcessor>();

            var processed = await processor.ProcessAsync(
                integrationEvent,
                CancellationToken.None);

            _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            _logger.LogInformation(
                processed
                    ? "Processed URL visit event {EventId} for {ShortCode}"
                    : "Ignored duplicate URL visit event {EventId} for {ShortCode}",
                integrationEvent.EventId,
                integrationEvent.ShortCode);
        }
        catch (JsonException exception)
        {
            _logger.LogError(exception, "Discarding an invalid URL visit event");
            _channel.BasicReject(eventArgs.DeliveryTag, requeue: false);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to process a URL visit event");
            _channel.BasicNack(
                eventArgs.DeliveryTag,
                multiple: false,
                requeue: true);
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        base.Dispose();
    }
}
