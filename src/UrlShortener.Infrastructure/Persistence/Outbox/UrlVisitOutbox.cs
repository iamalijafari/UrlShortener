using System.Text.Json;
using UrlShortener.Application.Abstractions.Messaging;
using UrlShortener.Application.IntegrationEvents;
using UrlShortener.Infrastructure.Persistence.Models;

namespace UrlShortener.Infrastructure.Persistence.Outbox;

public sealed class UrlVisitOutbox : IUrlVisitOutbox
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new(JsonSerializerDefaults.Web);

    private readonly AppDbContext _dbContext;

    public UrlVisitOutbox(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(UrlVisitedIntegrationEvent integrationEvent)
    {
        _dbContext.OutboxMessages.Add(new OutboxMessage
        {
            Id = integrationEvent.EventId,
            Type = nameof(UrlVisitedIntegrationEvent),
            Payload = JsonSerializer.Serialize(integrationEvent, SerializerOptions),
            OccurredAtUtc = integrationEvent.VisitedAtUtc
        });
    }
}
