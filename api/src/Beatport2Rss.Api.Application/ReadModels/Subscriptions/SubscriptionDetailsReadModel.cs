using Beatport2Rss.Api.Domain.Countries;
using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.Application.ReadModels.Subscriptions;

public sealed record SubscriptionDetailsReadModel(
    SubscriptionId Id,
    SubscriptionType Type,
    SubscriptionName Name,
    Slug Slug,
    BeatportId BeatportId,
    BeatportSlug BeatportSlug,
    Uri ImageUri,
    CountryName? Country,
    IEnumerable<SubscriptionTagDetailsReadModel> Tags);