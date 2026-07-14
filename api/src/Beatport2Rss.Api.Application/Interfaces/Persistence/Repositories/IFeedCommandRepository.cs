using Beatport2Rss.Api.Domain.Feeds;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;

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