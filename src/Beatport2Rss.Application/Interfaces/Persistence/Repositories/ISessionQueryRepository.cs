using Beatport2Rss.Application.Interfaces.Models.Sessions;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ISessionQueryRepository
{
    Task<bool> ExistsAsync(UserId userId, SessionId sessionId, CancellationToken cancellationToken = default);
    Task<IHaveSessionDetails> LoadSessionDetailsAsync(UserId userId, SessionId sessionId, CancellationToken cancellationToken = default);
}