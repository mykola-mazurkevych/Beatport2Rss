using Beatport2Rss.Common.Domain.Feeds;

namespace Beatport2Rss.Builder.Application.ReadModels.Feeds;

public sealed record FeedReadModel(
    FeedId Id,
    FeedName Name,
    AuthorName? AuthorName);
