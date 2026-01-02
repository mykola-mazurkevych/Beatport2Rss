using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SessionQueryRepository(Beatport2RssDbContext dbContext) :
    QueryRepository<Session, SessionId>(dbContext),
    ISessionQueryRepository
{
    protected override IQueryable<Session> ApplyIncludes(IQueryable<Session> query) => query;

    public async Task<IEnumerable<Session>> GetSessionsAsync(UserId userId, CancellationToken cancellationToken = default) =>
        await DbSet.Where(s => s.UserId == userId).ToListAsync(cancellationToken);
}