using Beatport2Rss.Collector.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

namespace Beatport2Rss.Collector.Application.Extensions;

internal static class FeedSubscriptionDeletedExtensions
{
    extension(FeedSubscriptionDeleted feedSubscriptionDeleted)
    {
        public DecrementSubscribersCountCommand ToCommand() =>
            new(feedSubscriptionDeleted.SubscriptionId);
    }
}
