using Beatport2Rss.Application.ReadModels.Feeds;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface IFeedQueryRepository : IQueryRepository
{
    Task<FeedDetailsReadModel> LoadFeedDetailsReadModelAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default);
}