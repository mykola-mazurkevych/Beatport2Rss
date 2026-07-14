using Beatport2Rss.Api.Domain.Countries;
using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;

internal sealed record SubscriptionQueryModel(
    SubscriptionId Id,
    DateTimeOffset CreatedAt,
    SubscriptionType Type,
    SubscriptionName Name,
    Slug Slug,
    BeatportId BeatportId,
    BeatportSlug BeatportSlug,
    Uri ImageUri,
    CountryName? Country,
    int SubscribersCount) :
    IQueryModel<SubscriptionId>
{
    public HashSet<SubscriptionTagQueryModel> Tags { get; set; } = [];
}