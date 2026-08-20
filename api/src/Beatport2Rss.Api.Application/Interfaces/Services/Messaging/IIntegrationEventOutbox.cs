using Beatport2Rss.Common.IntegrationEvents;

namespace Beatport2Rss.Api.Application.Interfaces.Services.Messaging;

public interface IIntegrationEventOutbox
{
    void Enqueue<TIntegrationEvent>(TIntegrationEvent integrationEvent)
        where TIntegrationEvent : IIntegrationEvent;
}