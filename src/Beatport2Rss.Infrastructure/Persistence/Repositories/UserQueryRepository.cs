using Beatport2Rss.Application.ReadModels.Users;
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

    public async Task<UserDetailsReadModel> LoadUserDetailsAsync(
        UserId userId,
        CancellationToken cancellationToken = default) =>
        await LoadAsync(
            userQueryModel => userQueryModel.Id == userId,
            userQueryModel => new UserDetailsReadModel(
                userQueryModel.EmailAddress,
                userQueryModel.FirstName,
                userQueryModel.LastName,
                userQueryModel.IsActive,
                userQueryModel.FeedsCount,
                userQueryModel.TagsCount),
            cancellationToken);

    public async Task<UserAuthDetailsReadModel> LoadUserAuthDetailsAsync(
        UserId userId,
        CancellationToken cancellationToken = default) =>
        await LoadAsync(
            userQueryModel => userQueryModel.Id == userId,
            userQueryModel => new UserAuthDetailsReadModel(
                userQueryModel.Id,
                userQueryModel.EmailAddress,
                userQueryModel.PasswordHash,
                userQueryModel.FirstName,
                userQueryModel.LastName),
            cancellationToken);

    public async Task<UserStatusDetailsReadModel> LoadUserStatusDetailsAsync(
        UserId userId,
        CancellationToken cancellationToken = default) =>
        await LoadAsync(
            userQueryModel => userQueryModel.Id == userId,
            userQueryModel => new UserStatusDetailsReadModel(userQueryModel.Status),
            cancellationToken);

    public async Task<UserAuthDetailsReadModel?> FindUserAuthDetailsAsync(
        EmailAddress emailAddress,
        CancellationToken cancellationToken = default) =>
        await FindAsync(
            userQueryModel => userQueryModel.EmailAddress == emailAddress,
            userQueryModel => new UserAuthDetailsReadModel(
                userQueryModel.Id,
                userQueryModel.EmailAddress,
                userQueryModel.PasswordHash,
                userQueryModel.FirstName,
                userQueryModel.LastName),
            cancellationToken);
}