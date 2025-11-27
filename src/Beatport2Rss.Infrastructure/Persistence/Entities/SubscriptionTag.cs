namespace Beatport2Rss.Infrastructure.Persistence.Entities;

public sealed class SubscriptionTag
{
    public int SubscriptionId { get; set; }
    public int TagId { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
}