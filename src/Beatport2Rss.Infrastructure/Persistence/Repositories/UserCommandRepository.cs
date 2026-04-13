using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class UserCommandRepository(DbSet<User> users) :
    CommandRepository<User, UserId>(users),
    IUserCommandRepository
{
    public Task<bool> ExistsAsync(
        EmailAddress emailAddress,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(
            user => user.EmailAddress == emailAddress,
            cancellationToken);

    public Task<bool> ExistsExceptAsync(
        EmailAddress emailAddress,
        UserId exceptUserId,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(
            user =>
                user.EmailAddress == emailAddress &&
                user.Id != exceptUserId,
            cancellationToken);

    public Task<User> LoadAsync(
        UserId userId,
        CancellationToken cancellationToken = default) =>
        LoadAsync(
            user => user.Id == userId,
            cancellationToken);

    Task IUserCommandRepository.AddAsync(
        User user,
        CancellationToken cancellationToken) =>
        AddAsync(user, cancellationToken);
}