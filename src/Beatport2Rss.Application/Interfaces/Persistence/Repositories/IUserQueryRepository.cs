using Beatport2Rss.Application.QueryModels.Users;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface IUserQueryRepository :
    IQueryRepository<UserQueryModel, UserId>
{
    Task<UserQueryModel?> FindAsync(
        EmailAddress emailAddress,
        CancellationToken cancellationToken = default) =>
        FindAsync(
            user => user.EmailAddress.Equals(emailAddress),
            cancellationToken);
}