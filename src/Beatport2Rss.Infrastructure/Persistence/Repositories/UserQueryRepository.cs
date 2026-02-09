using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.ReadModels.Users;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class UserQueryRepository(Beatport2RssDbContext dbContext) :
    IUserQueryRepository
{
    public Task<bool> ExistsAsync(UserId userId, CancellationToken cancellationToken = default) =>
        dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken);

    public Task<UserStatusReadModel> LoadUserStatusReadModelAsync(UserId userId, CancellationToken cancellationToken = default) =>
        GetUserStatusReadModelAsQueryable(userId).SingleAsync(cancellationToken);

    public Task<UserAuthDetailsReadModel> LoadUserAuthDetailsReadModelAsync(UserId userId, CancellationToken cancellationToken = default) =>
        GetUserAuthDetailsReadModelsAsQueryable(userId).SingleAsync(cancellationToken);

    public Task<UserAuthDetailsReadModel?> LoadUserAuthDetailsReadModelAsync(EmailAddress emailAddress, CancellationToken cancellationToken = default) =>
        GetUserAuthDetailsReadModelsAsQueryable(emailAddress).SingleOrDefaultAsync(cancellationToken);

    public Task<UserDetailsReadModel> LoadUserDetailsReadModelAsync(UserId userId, CancellationToken cancellationToken = default) =>
        GetUserDetailsReadModelsAsQueryable(userId).SingleAsync(cancellationToken);

    private IQueryable<UserStatusReadModel> GetUserStatusReadModelAsQueryable(UserId userId) =>
        from user in dbContext.Users
        where user.Id == userId
        select new UserStatusReadModel(user.Status);

    private IQueryable<UserAuthDetailsReadModel> GetUserAuthDetailsReadModelsAsQueryable(UserId userId) =>
        from user in dbContext.Users
        where user.Id == userId
        select new UserAuthDetailsReadModel(
            user.Id,
            user.EmailAddress,
            user.PasswordHash,
            user.FirstName,
            user.LastName);
    
    private IQueryable<UserAuthDetailsReadModel> GetUserAuthDetailsReadModelsAsQueryable(EmailAddress emailAddress) =>
        from user in dbContext.Users
        where user.EmailAddress == emailAddress
        select new UserAuthDetailsReadModel(
            user.Id,
            user.EmailAddress,
            user.PasswordHash,
            user.FirstName,
            user.LastName);

    private IQueryable<UserDetailsReadModel> GetUserDetailsReadModelsAsQueryable(UserId userId) =>
        from user in dbContext.Users
        where user.Id == userId
        select new UserDetailsReadModel(
            user.EmailAddress,
            user.FirstName,
            user.LastName,
            user.Status == UserStatus.Active,
            user.Feeds.Count,
            user.Tags.Count);
}