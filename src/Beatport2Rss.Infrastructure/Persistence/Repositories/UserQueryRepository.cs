using System.Linq.Expressions;

using Beatport2Rss.Application.Interfaces.Models.Users;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Persistence.QueryModels;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class UserQueryRepository(
    IQueryable<UserQueryModel> userQueryModels) :
    QueryRepository<UserQueryModel, UserId>(userQueryModels),
    IUserQueryRepository
{
    public Task<bool> ExistsAsync(
        UserId userId,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(
            userQueryModel => userQueryModel.Id == userId,
            cancellationToken);

    public async Task<IHaveUserDetails> LoadUserDetailsAsync(
        UserId userId,
        CancellationToken cancellationToken = default) =>
        await LoadAsync(
            userQueryModel => userQueryModel.Id == userId,
            UserDetails.Selector,
            cancellationToken);

    public async Task<IHaveUserAuthDetails> LoadUserAuthDetailsAsync(
        UserId userId,
        CancellationToken cancellationToken = default) =>
        await LoadAsync(
            userQueryModel => userQueryModel.Id == userId,
            UserAuthDetails.Selector,
            cancellationToken);

    public async Task<IHaveUserStatusDetails> LoadUserStatusDetailsAsync(
        UserId userId,
        CancellationToken cancellationToken = default) =>
        await LoadAsync(
            userQueryModel => userQueryModel.Id == userId,
            UserStatusDetails.Selector,
            cancellationToken);

    public async Task<IHaveUserAuthDetails?> FindUserAuthDetailsAsync(
        EmailAddress emailAddress,
        CancellationToken cancellationToken = default) =>
        await FindAsync(
            userQueryModel => userQueryModel.EmailAddress == emailAddress,
            UserAuthDetails.Selector,
            cancellationToken);

    private sealed record UserAuthDetails(
        UserId Id,
        EmailAddress EmailAddress,
        PasswordHash PasswordHash,
        string? FirstName,
        string? LastName) :
        IHaveUserAuthDetails
    {
        public static Expression<Func<UserQueryModel, UserAuthDetails>> Selector =>
            userQueryModel => new UserAuthDetails(
                userQueryModel.Id,
                userQueryModel.EmailAddress,
                userQueryModel.PasswordHash,
                userQueryModel.FirstName,
                userQueryModel.LastName);
    }

    private sealed record UserStatusDetails(
        UserStatus Status) :
        IHaveUserStatusDetails
    {
        public static Expression<Func<UserQueryModel, UserStatusDetails>> Selector =>
            userQueryModel => new UserStatusDetails(
                userQueryModel.Status);
    }

    private sealed record UserDetails(
        EmailAddress EmailAddress,
        string? FirstName,
        string? LastName,
        bool IsActive,
        int FeedsCount,
        int TagsCount) :
        IHaveUserDetails
    {
        public static Expression<Func<UserQueryModel, UserDetails>> Selector =>
            userQueryModel => new UserDetails(
                userQueryModel.EmailAddress,
                userQueryModel.FirstName,
                userQueryModel.LastName,
                userQueryModel.IsActive,
                userQueryModel.FeedsCount,
                userQueryModel.TagsCount);
    }
}