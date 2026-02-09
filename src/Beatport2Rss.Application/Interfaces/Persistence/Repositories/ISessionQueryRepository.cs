using Beatport2Rss.Application.ReadModels.Sessions;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ISessionQueryRepository :
    IQueryRepository
{
    Task<bool> ExistsAsync(UserId userId, SessionId sessionId, CancellationToken cancellationToken = default);
    Task<SessionDetailsReadModel> LoadSessionDetailsReadModelAsync(UserId userId, SessionId sessionId, CancellationToken cancellationToken = default);
}