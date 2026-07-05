using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Feeds;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Api.Application.ReadModels.Feeds;

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