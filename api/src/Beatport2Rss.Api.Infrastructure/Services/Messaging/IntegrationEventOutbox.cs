using System.Text.Json;

using Beatport2Rss.Api.Application.Interfaces.Services.Messaging;
using Beatport2Rss.Api.Infrastructure.Persistence;
using Beatport2Rss.Api.Infrastructure.Persistence.Outbox;
using Beatport2Rss.Common.IntegrationEvents;

using Microsoft.Extensions.Options;

namespace Beatport2Rss.Api.Infrastructure.Services.Messaging;

internal sealed class IntegrationEventOutbox(
    ApiDbContext dbContext,
    IOptions<JsonSerializerOptions> jsonSerializerOptions) :
    IIntegrationEventOutbox
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions.Value;

    public Task EnqueueAsync<TIntegrationEvent>(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
        where TIntegrationEvent : IIntegrationEvent
    {
        var message = OutboxMessage.Create(
            integrationEvent.EventId,
            integrationEvent.OccurredAt,
            type: typeof(TIntegrationEvent).Name,
            payload: JsonSerializer.Serialize(integrationEvent, _jsonSerializerOptions));
        return dbContext.OutboxMessages.AddAsync(message, cancellationToken).AsTask();
    }
}