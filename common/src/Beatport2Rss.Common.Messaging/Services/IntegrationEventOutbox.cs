using System.Text.Json;

using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.Messaging.Interfaces;
using Beatport2Rss.Common.Messaging.Persistence.Entities;
using Beatport2Rss.Common.Messaging.Persistence.Interfaces.Repositories;

using Microsoft.Extensions.Options;

namespace Beatport2Rss.Common.Messaging.Services;

internal sealed class IntegrationEventOutbox(
    IOutboxMessageRepository outboxMessageRepository,
    IOptions<JsonSerializerOptions> jsonSerializerOptions) :
    IIntegrationEventOutbox
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions.Value;

    public Task EnqueueAsync<TIntegrationEvent>(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
        where TIntegrationEvent : IIntegrationEvent
    {
        var outboxMessage = OutboxMessage.Create(
            integrationEvent.Id,
            integrationEvent.OccurredAt,
            type: typeof(TIntegrationEvent).Name,
            payload: JsonSerializer.Serialize(integrationEvent, _jsonSerializerOptions));
        return outboxMessageRepository.AddAsync(outboxMessage, cancellationToken);
    }
}