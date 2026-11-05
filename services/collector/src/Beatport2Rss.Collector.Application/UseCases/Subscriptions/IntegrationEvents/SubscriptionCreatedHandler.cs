using Beatport2Rss.Collector.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;

namespace Beatport2Rss.Collector.Application.UseCases.Subscriptions.IntegrationEvents;

public static class SubscriptionCreatedHandler
{
    public static CreateSubscriptionCommand Handle(SubscriptionCreated subscriptionCreated) =>
        subscriptionCreated.ToCommand();

    extension(SubscriptionCreated subscriptionCreated)
    {
        private CreateSubscriptionCommand ToCommand() =>
            new(subscriptionCreated.SubscriptionId,
                subscriptionCreated.OccurredAt,
                subscriptionCreated.Name,
                subscriptionCreated.Type,
                subscriptionCreated.BeatportId,
                subscriptionCreated.BeatportSlug);
    }
}