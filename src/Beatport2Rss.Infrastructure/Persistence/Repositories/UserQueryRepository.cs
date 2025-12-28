using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class UserQueryRepository(Beatport2RssDbContext dbContext) :
    QueryRepository<User, UserId>(dbContext),
    IUserQueryRepository
{
    protected override IQueryable<User> ApplyIncludes(IQueryable<User> query) =>
        query
            .Include(u => u.Feeds)
            .Include(u => u.Tags);
}