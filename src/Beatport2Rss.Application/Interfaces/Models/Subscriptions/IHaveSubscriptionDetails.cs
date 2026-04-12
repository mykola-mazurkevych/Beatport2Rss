using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.Application.Interfaces.Models.Subscriptions;

public interface IHaveSubscriptionDetails
{
    SubscriptionId Id { get; }
    SubscriptionName Name { get; }
    Slug Slug { get; }
    BeatportSubscriptionType BeatportType { get; }
    BeatportId BeatportId { get; }
    BeatportSlug BeatportSlug { get; }
    Uri ImageUri { get; }
    IEnumerable<IHaveSubscriptionTagDetails> Tags { get; }
    DateTimeOffset? CreatedAt { get; }
    DateTimeOffset? RefreshedAt { get; }
}