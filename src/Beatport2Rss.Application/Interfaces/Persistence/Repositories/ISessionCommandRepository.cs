using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ISessionCommandRepository
{
    Task<Session> LoadAsync(
        SessionId sessionId,
        CancellationToken cancellationToken = default);

    Task<Session?> FindAsync(
        SessionId sessionId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Session session,
        CancellationToken cancellationToken = default);

    void Update(Session session);

    void Delete(Session session);

    Task DeleteAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    Task DeleteExpiredAsync(CancellationToken cancellationToken = default);
}