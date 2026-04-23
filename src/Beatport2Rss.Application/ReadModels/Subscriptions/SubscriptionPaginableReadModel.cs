using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.ReadModels.Subscriptions;

public sealed record SubscriptionPaginableReadModel : IPaginable<SubscriptionId>
{
    public required SubscriptionId Id { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required SubscriptionName Name { get; init; }
    public required Slug Slug { get; init; }
    public required BeatportSubscriptionType BeatportType { get; init; }
    public required BeatportId BeatportId { get; init; }
    public required BeatportSlug BeatportSlug { get; init; }
    public required Uri ImageUri { get; init; }
    public required DateTimeOffset? RefreshedAt { get; init; }
    public required IEnumerable<SubscriptionTagDetailsReadModel> Tags { get; init; }
}