using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ISessionQueryRepository : IQueryRepository<Session, SessionId>
{
    Task<IEnumerable<Session>> GetSessionsAsync(UserId userId, CancellationToken cancellationToken = default);
}