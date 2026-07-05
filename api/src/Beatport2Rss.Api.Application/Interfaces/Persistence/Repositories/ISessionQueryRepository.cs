using Beatport2Rss.Api.Application.ReadModels.Sessions;
using Beatport2Rss.Api.Domain.Sessions;
using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;

public interface ISessionQueryRepository
{
    Task<bool> ExistsAsync(
        UserId userId,
        SessionId sessionId,
        CancellationToken cancellationToken = default);

    Task<SessionDetailsReadModel> LoadSessionDetailsAsync(
        UserId userId,
        SessionId sessionId,
        CancellationToken cancellationToken = default);
}