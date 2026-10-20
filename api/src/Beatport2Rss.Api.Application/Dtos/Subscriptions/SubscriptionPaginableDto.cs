using Beatport2Rss.Api.Domain.Countries;
using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.ValueObjects;

namespace Beatport2Rss.Api.Application.Dtos.Subscriptions;

public sealed record SubscriptionPaginableDto(
    SubscriptionId Id,
    SubscriptionType Type,
    SubscriptionName Name,
    Slug Slug,
    Uri BeatportUri,
    Uri ImageUri,
    CountryName? Country,
    int SubscribersCount,
    IEnumerable<SubscriptionTagDto> Tags);