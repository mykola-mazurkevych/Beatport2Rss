using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;

namespace Beatport2Rss.Application.Dtos.Feeds;

public sealed record FeedPageDto(
    FeedId Id,
    FeedName Name,
    Slug Slug,
    bool IsActive,
    DateTimeOffset CreatedAt,
    int SubscriptionsCount);