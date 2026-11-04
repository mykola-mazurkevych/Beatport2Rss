using Beatport2Rss.Collector.Application.Extensions;
using Beatport2Rss.Collector.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;

namespace Beatport2Rss.Collector.Application.UseCases.Subscriptions.IntegrationEvents;

public static class SubscriptionCreatedHandler
{
    public static CreateSubscriptionCommand Handle(SubscriptionCreated subscriptionCreated) =>
        subscriptionCreated.ToCommand();
}