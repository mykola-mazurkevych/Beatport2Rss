using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.Application.ReadModels.Subscriptions;

public sealed record SubscriptionDetailsReadModel(
    SubscriptionId Id,
    SubscriptionName Name,
    Slug Slug,
    BeatportSubscriptionType BeatportType,
    BeatportId BeatportId,
    BeatportSlug BeatportSlug,
    Uri ImageUri,
    IEnumerable<SubscriptionTagDetailsReadModel> Tags,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? RefreshedAt);