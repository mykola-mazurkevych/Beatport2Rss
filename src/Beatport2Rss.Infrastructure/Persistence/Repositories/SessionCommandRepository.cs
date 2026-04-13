using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SessionCommandRepository(
    IClock clock,
    DbSet<Session> sessions) :
    CommandRepository<Session, SessionId>(sessions),
    ISessionCommandRepository
{
    public Task<Session> LoadAsync(
        SessionId sessionId,
        CancellationToken cancellationToken = default) =>
        LoadAsync(
            session => session.Id == sessionId,
            cancellationToken);

    public Task<Session?> FindAsync(
        SessionId sessionId,
        CancellationToken cancellationToken = default) =>
        FindAsync(
            session => session.Id == sessionId,
            cancellationToken);

    Task ISessionCommandRepository.AddAsync(
        Session session,
        CancellationToken cancellationToken) =>
        AddAsync(session, cancellationToken);

    public Task DeleteAsync(
        UserId userId,
        CancellationToken cancellationToken = default) =>
        DeleteAsync(
            session => session.UserId == userId,
            cancellationToken);

    public Task DeleteExpiredAsync(CancellationToken cancellationToken = default) =>
        DeleteAsync(
            session => session.RefreshTokenExpiresAt < clock.UtcNow,
            cancellationToken);
}