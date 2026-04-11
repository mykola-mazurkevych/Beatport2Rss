using Beatport2Rss.Application.Interfaces.Models.Users;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface IUserQueryRepository
{
    Task<bool> ExistsAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    Task<IHaveUserDetails> LoadUserDetailsAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    Task<IHaveUserAuthDetails> LoadUserAuthDetailsAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    Task<IHaveUserStatusDetails> LoadUserStatusDetailsAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    Task<IHaveUserAuthDetails?> FindUserAuthDetailsAsync(
        EmailAddress emailAddress,
        CancellationToken cancellationToken = default);
}