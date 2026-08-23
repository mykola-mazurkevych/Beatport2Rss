using Beatport2Rss.Common.IntegrationEvents;

namespace Beatport2Rss.Api.Application.Interfaces.Services.Messaging;

public interface IIntegrationEventOutbox
{
    Task EnqueueAsync<TIntegrationEvent>(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
        where TIntegrationEvent : IIntegrationEvent;
}