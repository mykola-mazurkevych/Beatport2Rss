using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Sessions;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SessionQueryRepository(Beatport2RssDbContext dbContext) :
    QueryRepository<Session, SessionId>(dbContext),
    ISessionQueryRepository
{
    protected override IQueryable<Session> ApplyIncludes(IQueryable<Session> query) => query;
}