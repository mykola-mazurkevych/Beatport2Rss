using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;

namespace Beatport2Rss.Application.ReadModels.Feeds;

public sealed record FeedDetailsReadModel(
    FeedId FeedId,
    Slug Slug,
    string Name,
    string? Owner,
    bool IsActive,
    DateTimeOffset CreatedAt);