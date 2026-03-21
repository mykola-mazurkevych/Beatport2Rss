using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface IFeedCommandRepository :
    ICommandRepository<Feed, FeedId>
{
    Task<Feed> LoadWithSubscriptionsAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default);
}