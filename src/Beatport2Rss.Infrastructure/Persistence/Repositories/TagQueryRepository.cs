using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class TagQueryRepository(Beatport2RssDbContext dbContext) :
    ITagQueryRepository
{
    public Task<bool> ExistsAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default) =>
        dbContext.Tags.AnyAsync(
            t =>
                t.UserId == userId &&
                t.Slug == slug,
            cancellationToken);
}