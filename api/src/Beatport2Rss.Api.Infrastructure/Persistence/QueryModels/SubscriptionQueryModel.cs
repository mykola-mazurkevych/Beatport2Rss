using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Countries;
using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;

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