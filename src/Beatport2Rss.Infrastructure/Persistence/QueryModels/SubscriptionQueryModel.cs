using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Countries;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.SharedKernel.Interfaces;

namespace Beatport2Rss.Infrastructure.Persistence.QueryModels;

internal sealed record SubscriptionQueryModel(
    SubscriptionId Id,
    DateTimeOffset CreatedAt,
    SubscriptionName Name,
    Slug Slug,
    BeatportSubscriptionType BeatportType,
    BeatportId BeatportId,
    BeatportSlug BeatportSlug,
    Uri ImageUri,
    CountryName? Country,
    int SubscribersCount) :
    IQueryModel<SubscriptionId>
{
    public HashSet<SubscriptionTagQueryModel> Tags { get; set; } = [];
}