using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class UserCommandRepository(Beatport2RssDbContext dbContext) :
    CommandRepository<User, UserId>(dbContext),
    IUserCommandRepository
{
    public Task<User> LoadWithFeedsAsync(UserId userId, CancellationToken cancellationToken = default) =>
        Entities
            .Include(u => u.Feeds)
            .SingleAsync(u => u.Id == userId, cancellationToken);

    public Task<User> LoadWithTagsAsync(UserId userId, CancellationToken cancellationToken = default) =>
        Entities
            .Include(u => u.Tags)
            .SingleAsync(u => u.Id == userId, cancellationToken);
}