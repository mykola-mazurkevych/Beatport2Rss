using Beatport2Rss.Builder.Domain.Users;
using Beatport2Rss.Common.Domain.Users;

namespace Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;

public interface IUserCommandRepository
{
    Task<bool> ExistsAsync(UserId userId, CancellationToken cancellationToken = default);

    Task<User?> FindAsync(UserId userId, CancellationToken cancellationToken = default);

    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
    void Delete(User user);
}