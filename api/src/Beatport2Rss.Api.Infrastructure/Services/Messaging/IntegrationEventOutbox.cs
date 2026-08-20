using System.Text.Json;

using Beatport2Rss.Api.Application.Interfaces.Services.Messaging;
using Beatport2Rss.Api.Infrastructure.Persistence;
using Beatport2Rss.Api.Infrastructure.Persistence.Outbox;
using Beatport2Rss.Common.IntegrationEvents;

namespace Beatport2Rss.Api.Infrastructure.Services.Messaging;

internal sealed class IntegrationEventOutbox(
    ApiDbContext dbContext,
    JsonSerializerOptions jsonSerializerOptions) :
    IIntegrationEventOutbox
{
    public void Enqueue<TIntegrationEvent>(TIntegrationEvent integrationEvent)
        where TIntegrationEvent : IIntegrationEvent
    {
        var message = OutboxMessage.Create(
            integrationEvent.EventId,
            integrationEvent.OccurredAt,
            type: typeof(TIntegrationEvent).Name,
            payload: JsonSerializer.Serialize(integrationEvent, jsonSerializerOptions));
        dbContext.OutboxMessages.Add(message);
    }
}