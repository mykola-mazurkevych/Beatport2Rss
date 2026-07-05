using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Countries;
using Beatport2Rss.Api.Domain.Subscriptions;

namespace Beatport2Rss.Api.Application.ReadModels.Subscriptions;

public sealed record SubscriptionDetailsReadModel(
    SubscriptionId Id,
    SubscriptionName Name,
    Slug Slug,
    BeatportSubscriptionType BeatportType,
    BeatportId BeatportId,
    BeatportSlug BeatportSlug,
    Uri ImageUri,
    CountryName? Country,
    IEnumerable<SubscriptionTagDetailsReadModel> Tags);