using Beatport2Rss.Collector.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;

namespace Beatport2Rss.Collector.Application.UseCases.Subscriptions.IntegrationEvents;

public static class SubscriptionRequestedHandler
{
    public static CreateSubscriptionCommand Handle(SubscriptionRequested subscriptionRequested) =>
        new(subscriptionRequested.Type,
            subscriptionRequested.BeatportId);
}