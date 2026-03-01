using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Feeds;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class FeedCommandRepository(Beatport2RssDbContext dbContext) :
    CommandRepository<Feed, FeedId>(dbContext),
    IFeedCommandRepository;