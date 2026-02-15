using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;

namespace Beatport2Rss.Application.Dtos.Feeds;

public sealed record FeedDto(
    FeedId Id,
    Slug Slug,
    FeedName Name,
    string? Owner,
    bool IsActive,
    DateTimeOffset CreatedAt);