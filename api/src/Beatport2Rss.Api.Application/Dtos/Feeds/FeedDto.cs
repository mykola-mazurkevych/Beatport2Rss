using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Feeds;

namespace Beatport2Rss.Api.Application.Dtos.Feeds;

public sealed record FeedDto(
    FeedId Id,
    FeedName Name,
    Slug Slug,
    bool IsActive,
    DateTimeOffset CreatedAt,
    int SubscriptionsCount);