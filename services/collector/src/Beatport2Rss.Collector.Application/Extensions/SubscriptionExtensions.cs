using Beatport2Rss.Collector.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;

namespace Beatport2Rss.Collector.Application.Extensions;

internal static class SubscriptionExtensions
{
    extension(SubscriptionCreated subscriptionCreated)
    {
        public CreateSubscriptionCommand ToCommand() =>
            new(SubscriptionId.Create(subscriptionCreated.SubscriptionId),
                subscriptionCreated.OccurredAt,
                SubscriptionName.Create(subscriptionCreated.Name),
                subscriptionCreated.Type,
                BeatportId.Create(subscriptionCreated.BeatportId),
                BeatportSlug.Create(subscriptionCreated.BeatportSlug));
    }
}