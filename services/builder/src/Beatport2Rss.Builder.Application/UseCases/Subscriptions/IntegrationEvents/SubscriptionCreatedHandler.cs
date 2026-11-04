using Beatport2Rss.Builder.Application.Extensions;
using Beatport2Rss.Builder.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;

namespace Beatport2Rss.Builder.Application.UseCases.Subscriptions.IntegrationEvents;

public static class SubscriptionCreatedHandler
{
    public static CreateSubscriptionCommand Handle(SubscriptionCreated subscriptionCreated) =>
        subscriptionCreated.ToCommand();
}