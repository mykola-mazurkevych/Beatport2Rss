using Beatport2Rss.Application.Interfaces.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class UserQueryRepository(Beatport2RssDbContext dbContext)
    : QueryRepository<Domain.Users.User, Domain.Users.UserId>(dbContext), IUserQueryRepository
{
    protected override IQueryable<Domain.Users.User> ApplyIncludes(IQueryable<Domain.Users.User> query) =>
        query
            .Include(u => u.Feeds)
            .Include(u => u.Tags);
}