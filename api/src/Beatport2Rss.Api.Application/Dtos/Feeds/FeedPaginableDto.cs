using Beatport2Rss.Api.Domain.Feeds;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.Application.Dtos.Feeds;

public sealed record FeedPaginableDto(
    FeedId Id,
    FeedName Name,
    Slug Slug,
    AuthorName? AuthorName,
    bool IsActive,
    DateTimeOffset CreatedAt,
    int SubscriptionsCount);