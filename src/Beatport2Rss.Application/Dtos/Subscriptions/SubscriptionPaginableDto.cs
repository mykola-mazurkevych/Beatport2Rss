using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.Application.Dtos.Subscriptions;

public sealed record SubscriptionPaginableDto(
    SubscriptionId Id,
    SubscriptionName Name,
    Slug Slug,
    BeatportSubscriptionType BeatportType,
    Uri BeatportUri,
    Uri ImageUri,
    IEnumerable<SubscriptionTagDto> Tags,
    DateTimeOffset? RefreshedAt);