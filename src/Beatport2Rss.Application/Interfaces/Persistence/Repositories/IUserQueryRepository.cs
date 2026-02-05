using Beatport2Rss.Application.ReadModels.Users;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface IUserQueryRepository : IQueryRepository
{
    Task<bool> ExistsAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<UserStatusReadModel?> LoadUserStatusQueryModelAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<UserAuthDetailsReadModel> LoadUserAuthDetailsReadModelAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<UserAuthDetailsReadModel?> LoadUserAuthDetailsReadModelAsync(EmailAddress emailAddress, CancellationToken cancellationToken = default);
    Task<UserDetailsReadModel> LoadUserDetailsReadModelAsync(UserId userId, CancellationToken cancellationToken = default);
}