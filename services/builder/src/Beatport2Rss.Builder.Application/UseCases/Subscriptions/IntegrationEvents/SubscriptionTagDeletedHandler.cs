using Beatport2Rss.Builder.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;

namespace Beatport2Rss.Builder.Application.UseCases.Subscriptions.IntegrationEvents;

public static class SubscriptionTagDeletedHandler
{
    public static DeleteSubscriptionTagCommand Handle(SubscriptionTagDeleted subscriptionTagDeleted) =>
        new(subscriptionTagDeleted.SubscriptionId,
            subscriptionTagDeleted.TagId);
}