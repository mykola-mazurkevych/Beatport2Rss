using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Feeds;

namespace Beatport2Rss.Api.Application.Dtos.Feeds;

public sealed record FeedDto(
    FeedId Id,
    FeedName Name,
    Slug Slug,
    AuthorName? AuthorName,
    bool IsActive,
    DateTimeOffset CreatedAt,
    int SubscriptionsCount);