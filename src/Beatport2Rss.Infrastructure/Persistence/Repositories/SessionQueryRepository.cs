using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Application.ReadModels.Sessions;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SessionQueryRepository(
    IClock clock,
    IQueryable<Session> sessions,
    IQueryable<User> users) :
    ISessionQueryRepository
{
    public Task<bool> ExistsAsync(UserId userId, SessionId sessionId, CancellationToken cancellationToken = default) =>
        sessions
            .AnyAsync(s => s.UserId == userId && s.Id == sessionId, cancellationToken);

    public Task<SessionDetailsReadModel> LoadSessionDetailsReadModelAsync(UserId userId, SessionId sessionId, CancellationToken cancellationToken = default) =>
        (
            from session in sessions
            join user in users on session.UserId equals user.Id
            where session.UserId == userId &&
                  session.Id == sessionId
            select new SessionDetailsReadModel(
                session.Id,
                session.CreatedAt,
                user.EmailAddress,
                user.FirstName,
                user.LastName,
                session.UserAgent,
                session.IpAddress,
                session.RefreshTokenExpiresAt < clock.UtcNow)
        )
        .SingleAsync(cancellationToken);
}