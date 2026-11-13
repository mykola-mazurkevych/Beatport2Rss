using Beatport2Rss.Builder.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;

namespace Beatport2Rss.Builder.Application.UseCases.Subscriptions.IntegrationEvents;

public static class SubscriptionTagCreatedHandler
{
    public static CreateSubscriptionTagCommand Handle(SubscriptionTagCreated subscriptionTagCreated) =>
        new(subscriptionTagCreated.SubscriptionId,
            subscriptionTagCreated.TagId);
}
