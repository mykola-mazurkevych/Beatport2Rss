using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;

namespace Beatport2Rss.Application.Interfaces.Models.Feeds;

public interface IHaveFeedDetails
{
    FeedId Id { get; }
    FeedName Name { get; }
    Slug Slug { get; }
    bool IsActive { get; }
    DateTimeOffset CreatedAt { get; }
    int SubscriptionsCount { get; }
}