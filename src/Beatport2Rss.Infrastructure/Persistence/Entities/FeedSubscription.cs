namespace Beatport2Rss.Infrastructure.Persistence.Entities;

public sealed class FeedSubscription
{
    public Guid FeedId { get; set; }
    public int SubscriptionId { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
}