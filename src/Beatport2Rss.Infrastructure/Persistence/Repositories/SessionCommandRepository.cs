using System.Linq.Expressions;

using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Sessions;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SessionCommandRepository(DbSet<Session> sessions) :
    CommandRepository<Session, SessionId>(sessions),
    ISessionCommandRepository
{
    private readonly DbSet<Session> _sessions = sessions;

    public Task ExecuteDeleteAsync(Expression<Func<Session, bool>> predicate, CancellationToken cancellationToken) =>
        _sessions
            .Where(predicate)
            .ExecuteDeleteAsync(cancellationToken);
}