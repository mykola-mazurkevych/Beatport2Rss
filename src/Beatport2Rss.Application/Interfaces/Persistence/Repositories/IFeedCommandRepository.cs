using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface IFeedCommandRepository
{
    Task<bool> ExistsAsync(
        UserId userId,
        FeedName feedName,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsExceptAsync(
        UserId userId,
        FeedName feedName,
        FeedId exceptFeedId,
        CancellationToken cancellationToken = default);

    Task<Feed> LoadAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default);

    Task<Feed> LoadWithSubscriptionsAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Feed feed,
        CancellationToken cancellationToken = default);

    void Update(Feed feed);

    void Delete(Feed feed);
}