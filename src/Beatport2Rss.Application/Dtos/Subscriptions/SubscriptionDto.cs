using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.Application.Dtos.Subscriptions;

public sealed record SubscriptionDto(
    SubscriptionId Id,
    string Name,
    BeatportId BeatportId,
    BeatportSlug BeatportSlug,
    Uri BeatportUri,
    Uri ImageUri,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? RefreshedAt);