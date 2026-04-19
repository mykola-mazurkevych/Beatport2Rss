using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.ReadModels.Feeds;

public sealed record FeedPaginableReadModel :
    IPaginable<FeedId>
{
    public required FeedId Id { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required FeedName Name { get; init; }
    public required Slug Slug { get; init; }
    public required bool IsActive { get; init; }
    public required int SubscriptionsCount { get; init; }
}