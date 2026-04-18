using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.ReadModels.Feeds;

public sealed record FeedPageReadModel(
    FeedId Id,
    DateTimeOffset CreatedAt,
    FeedName Name,
    Slug Slug,
    bool IsActive,
    int SubscriptionsCount) :
    IPaginable<FeedId>;