using Beatport2Rss.Collector.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

namespace Beatport2Rss.Collector.Application.Extensions;

internal static class FeedSubscriptionCreatedExtensions
{
    extension(FeedSubscriptionCreated feedSubscriptionCreated)
    {
        public IncrementSubscribersCountCommand ToCommand() =>
            new(feedSubscriptionCreated.SubscriptionId);
    }
}