using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.Infrastructure.Persistence.Entities;

public sealed class FeedSubscription
{
    public FeedId FeedId { get; set; }
    public SubscriptionId SubscriptionId { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
}