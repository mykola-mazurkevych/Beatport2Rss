using Beatport2Rss.Api.Domain.Countries;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Subscriptions;

namespace Beatport2Rss.Api.Application.Dtos.Subscriptions;

public sealed record SubscriptionDto(
    SubscriptionId Id,
    SubscriptionType Type,
    SubscriptionName Name,
    Slug Slug,
    Uri BeatportUri,
    Uri ImageUri,
    CountryName? Country,
    IEnumerable<SubscriptionTagDto> Tags);