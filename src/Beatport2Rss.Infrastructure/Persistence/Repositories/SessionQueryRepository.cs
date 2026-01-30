using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Application.ReadModels.Sessions;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SessionQueryRepository(
    IClock clock,
    Beatport2RssDbContext dbContext) :
    ISessionQueryRepository
{
    public Task<bool> ExistsAsync(UserId userId, SessionId sessionId, CancellationToken cancellationToken = default) =>
        dbContext.Sessions
            .AnyAsync(
                session =>
                    session.UserId == userId &&
                    session.Id == sessionId,
                cancellationToken);

    public Task<SessionDetailsReadModel> LoadSessionDetailsQueryModelAsync(UserId userId, SessionId sessionId, CancellationToken cancellationToken = default) =>
        GetSessionDetailsReadModelsAsQueryable(userId, sessionId).SingleAsync(cancellationToken);

    private IQueryable<SessionDetailsReadModel> GetSessionDetailsReadModelsAsQueryable(UserId userId, SessionId sessionId) =>
        from session in dbContext.Sessions
        join user in dbContext.Users on session.UserId equals user.Id
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
            session.RefreshTokenExpiresAt < clock.UtcNow);
}