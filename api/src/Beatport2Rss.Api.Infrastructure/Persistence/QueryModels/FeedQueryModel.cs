using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Feeds;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;

internal sealed record FeedQueryModel(
    FeedId Id,
    DateTimeOffset CreatedAt,
    UserId UserId,
    FeedName Name,
    Slug Slug,
    bool IsActive,
    int SubscriptionsCount) :
    IQueryModel<FeedId>;