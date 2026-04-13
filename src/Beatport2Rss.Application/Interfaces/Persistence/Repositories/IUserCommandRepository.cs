using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface IUserCommandRepository
{
    Task<bool> ExistsAsync(
        EmailAddress emailAddress,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsExceptAsync(
        EmailAddress emailAddress,
        UserId exceptUserId,
        CancellationToken cancellationToken = default);

    Task<User> LoadAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken = default);

    void Update(User user);

    void Delete(User user);
}