using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Domain.Feeds;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Repositories;

internal sealed class FeedCommandRepository(DbSet<Feed> feeds) :
    CommandRepository<Feed, FeedId>(feeds),
    IFeedCommandRepository
{
    public Task<bool> ExistsAsync(FeedId feedId, CancellationToken cancellationToken = default) =>
        ExistsAsync(feed => feed.Id == feedId, cancellationToken);
}