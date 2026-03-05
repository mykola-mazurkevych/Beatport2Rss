using System.Linq.Expressions;

using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Sessions;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SessionCommandRepository(Beatport2RssDbContext dbContext) :
    CommandRepository<Session, SessionId>(dbContext),
    ISessionCommandRepository
{
    private readonly Beatport2RssDbContext _dbContext = dbContext;

    public Task ExecuteDeleteAsync(Expression<Func<Session, bool>> predicate, CancellationToken cancellationToken) =>
        _dbContext.Sessions
            .Where(predicate)
            .ExecuteDeleteAsync(cancellationToken);
}