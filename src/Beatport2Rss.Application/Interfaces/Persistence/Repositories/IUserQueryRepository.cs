using Beatport2Rss.Application.Interfaces.Models.Users;
using Beatport2Rss.Application.QueryModels.Users;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface IUserQueryRepository :
    IQueryRepository<UserQueryModel, UserId>
{
    Task<IHaveUserAuth> LoadUserAuthAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    Task<IHaveUserStatus> LoadUserStatusAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    Task<IHaveUserAuth?> FindUserAuthAsync(
        EmailAddress emailAddress,
        CancellationToken cancellationToken = default);
}