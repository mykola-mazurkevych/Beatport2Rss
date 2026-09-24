using Beatport2Rss.Common.IntegrationEvents;

namespace Beatport2Rss.Common.Messaging.Interfaces;

public interface IIntegrationEventOutbox
{
    Task EnqueueAsync<TIntegrationEvent>(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
        where TIntegrationEvent : IIntegrationEvent;
}