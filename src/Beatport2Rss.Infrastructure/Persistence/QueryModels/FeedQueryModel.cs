using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Infrastructure.Persistence.QueryModels;

internal sealed record FeedQueryModel(
    FeedId Id,
    DateTimeOffset CreatedAt,
    UserId UserId,
    FeedName Name,
    Slug Slug,
    bool IsActive,
    int SubscriptionsCount) :
    IQueryModel<FeedId>;