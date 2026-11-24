using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Repositories;

internal sealed class UserCommandRepository(DbSet<User> users, ApiDbContext dbContext) :
    CommandRepository<User, UserId>(users),
    IUserCommandRepository
{
    public async Task<bool> ActivatePendingAsync(
        UserId userId,
        EmailAddress emailAddress,
        CancellationToken cancellationToken = default) =>
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""UPDATE api."Users" SET "Status" = {UserStatus.Active.ToString()} WHERE "Id" = {userId.Value} AND "Status" = {UserStatus.Pending.ToString()} AND "EmailAddress" COLLATE "C" = {emailAddress.Value} COLLATE "C" """,
            cancellationToken) > 0;

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