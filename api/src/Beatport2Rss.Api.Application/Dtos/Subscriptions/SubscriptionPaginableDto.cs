using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Countries;
using Beatport2Rss.Api.Domain.Subscriptions;

namespace Beatport2Rss.Api.Application.Dtos.Subscriptions;

public sealed record SubscriptionPaginableDto(
    SubscriptionId Id,
    SubscriptionName Name,
    Slug Slug,
    BeatportSubscriptionType BeatportType,
    Uri BeatportUri,
    Uri ImageUri,
    CountryName? Country,
    int SubscribersCount,
    IEnumerable<SubscriptionTagDto> Tags);