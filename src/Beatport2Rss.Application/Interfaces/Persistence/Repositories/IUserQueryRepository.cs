using Beatport2Rss.Application.ReadModels.Users;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface IUserQueryRepository
{
    Task<bool> ExistsAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    Task<UserDetailsReadModel> LoadUserDetailsAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    Task<UserAuthDetailsReadModel> LoadUserAuthDetailsAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    Task<UserStatusDetailsReadModel> LoadUserStatusDetailsAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    Task<UserAuthDetailsReadModel?> FindUserAuthDetailsAsync(
        EmailAddress emailAddress,
        CancellationToken cancellationToken = default);
}