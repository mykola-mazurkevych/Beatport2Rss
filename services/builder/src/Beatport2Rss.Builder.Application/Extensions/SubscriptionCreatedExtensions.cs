using Beatport2Rss.Builder.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;

namespace Beatport2Rss.Builder.Application.Extensions;

internal static class SubscriptionCreatedExtensions
{
    extension(SubscriptionCreated subscriptionCreated)
    {
        public CreateSubscriptionCommand ToCommand() =>
            new(subscriptionCreated.SubscriptionId,
                subscriptionCreated.OccurredAt,
                subscriptionCreated.Name,
                subscriptionCreated.Type,
                subscriptionCreated.BeatportId);
    }
}