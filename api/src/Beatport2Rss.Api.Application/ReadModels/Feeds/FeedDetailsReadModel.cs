using Beatport2Rss.Api.Domain.Feeds;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.Application.ReadModels.Feeds;

public sealed record FeedDetailsReadModel(
    FeedId Id,
    FeedName Name,
    Slug Slug,
    bool IsActive,
    DateTimeOffset CreatedAt,
    int SubscriptionsCount);