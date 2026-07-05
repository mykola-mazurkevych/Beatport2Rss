using Beatport2Rss.Api.Domain.Sessions;
using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;

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