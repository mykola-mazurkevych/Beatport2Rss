using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Countries;
using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Api.Application.ReadModels.Subscriptions;

public sealed record SubscriptionPaginableReadModel : IPaginable<SubscriptionId>
{
    public required SubscriptionId Id { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required SubscriptionType Type { get; init; }
    public required SubscriptionName Name { get; init; }
    public required Slug Slug { get; init; }
    public required BeatportId BeatportId { get; init; }
    public required BeatportSlug BeatportSlug { get; init; }
    public required Uri ImageUri { get; init; }
    public required CountryName? Country { get; init; }
    public required int SubscribersCount { get; init; }
    public required IEnumerable<SubscriptionTagDetailsReadModel> Tags { get; init; }
}