using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Feeds;

namespace Beatport2Rss.Api.Application.ReadModels.Feeds;

public sealed record FeedDetailsReadModel(
    FeedId Id,
    FeedName Name,
    Slug Slug,
    AuthorName? AuthorName,
    bool IsActive,
    DateTimeOffset CreatedAt,
    int SubscriptionsCount);