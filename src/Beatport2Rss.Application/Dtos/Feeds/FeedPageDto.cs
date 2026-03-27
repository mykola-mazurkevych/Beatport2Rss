using System.Linq.Expressions;

using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.Dtos.Feeds;

public sealed record FeedPageDto(
    FeedId Id,
    FeedName Name,
    Slug Slug,
    bool IsActive,
    DateTimeOffset CreatedAt,
    int SubscriptionsCount) :
    IPageDto<FeedId>
{
    public static Expression<Func<Feed, FeedPageDto>> Create =>
        feed => new FeedPageDto(
            feed.Id,
            feed.Name,
            feed.Slug,
            feed.IsActive,
            feed.CreatedAt,
            feed.Subscriptions.Count);
}