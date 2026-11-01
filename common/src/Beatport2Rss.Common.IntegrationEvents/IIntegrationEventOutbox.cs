namespace Beatport2Rss.Common.IntegrationEvents;

public interface IIntegrationEventOutbox
{
    Task EnqueueAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}