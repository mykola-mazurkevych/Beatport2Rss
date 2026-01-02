using Beatport2Rss.Application.Interfaces.Services.Checkers;
using Beatport2Rss.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Services.Checkers;

internal sealed class FeedNameAvailabilityChecker(Beatport2RssDbContext dbContext) :
    IFeedNameAvailabilityChecker
{
    Task<bool> IFeedNameAvailabilityChecker.IsAvailableAsync(Guid userId, string name, CancellationToken cancellationToken) =>
        dbContext.Feeds
            .Where(f => f.UserId == userId)
            .AllAsync(f => f.Name != name, cancellationToken);
}