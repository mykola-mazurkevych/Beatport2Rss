using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tags;

namespace Beatport2Rss.Infrastructure.Persistence.Entities;

public sealed class SubscriptionTag
{
    public SubscriptionId SubscriptionId { get; set; }
    public TagId TagId { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
}