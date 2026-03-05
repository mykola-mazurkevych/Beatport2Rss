using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Feeds;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class FeedCommandRepository(DbSet<Feed> feeds) :
    CommandRepository<Feed, FeedId>(feeds),
    IFeedCommandRepository;