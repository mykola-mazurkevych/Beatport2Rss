using System.Linq.Expressions;

using Beatport2Rss.Application.Interfaces.Models.Users;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.QueryModels.Users;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class UserQueryRepository(
    IQueryable<UserQueryModel> userQueryModels) :
    QueryRepository<UserQueryModel, UserId>(userQueryModels),
    IUserQueryRepository
{
    private static Expression<Func<UserQueryModel, UserAuthProjection>> UserAuthSelector =>
        userQueryModel => new UserAuthProjection(
            userQueryModel.Id,
            userQueryModel.EmailAddress,
            userQueryModel.PasswordHash,
            userQueryModel.FirstName,
            userQueryModel.LastName);

    private static Expression<Func<UserQueryModel, UserStatusProjection>> UserStatusSelector =>
        userQueryModel => new UserStatusProjection(
            userQueryModel.Status);

    public async Task<IHaveUserAuth> LoadUserAuthAsync(
        UserId userId,
        CancellationToken cancellationToken = default) =>
        await LoadAsync(
            userQueryModel => userQueryModel.Id.Equals(userId),
            UserAuthSelector,
            cancellationToken);

    public async Task<IHaveUserStatus> LoadUserStatusAsync(
        UserId userId,
        CancellationToken cancellationToken = default) =>
        await LoadAsync(
            userQueryModel => userQueryModel.Id.Equals(userId),
            UserStatusSelector,
            cancellationToken);

    public async Task<IHaveUserAuth?> FindUserAuthAsync(
        EmailAddress emailAddress,
        CancellationToken cancellationToken = default) =>
        await FindAsync(
            userQueryModel => userQueryModel.EmailAddress.Equals(emailAddress),
            UserAuthSelector,
            cancellationToken);

    private sealed record UserAuthProjection(
        UserId Id,
        EmailAddress EmailAddress,
        PasswordHash PasswordHash,
        string? FirstName,
        string? LastName) :
        IHaveUserAuth;

    private sealed record UserStatusProjection(
        UserStatus Status) :
        IHaveUserStatus;
}