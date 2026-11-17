using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Domain.Feeds;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Repositories;

internal sealed class FeedCommandRepository(DbSet<Feed> feeds) :
    CommandRepository<Feed, FeedId>(feeds),
    IFeedCommandRepository;