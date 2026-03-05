using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.ReadModels.Users;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class UserQueryRepository(
    IQueryable<Feed> feeds,
    IQueryable<Tag> tags,
    IQueryable<User> users) :
    IUserQueryRepository
{
    public Task<bool> ExistsAsync(UserId userId, CancellationToken cancellationToken = default) =>
        users.AnyAsync(u => u.Id == userId, cancellationToken);

    public Task<UserStatusReadModel> LoadUserStatusReadModelAsync(UserId userId, CancellationToken cancellationToken = default) =>
        (
            from user in users
            where user.Id == userId
            select new UserStatusReadModel(user.Status)
        )
        .SingleAsync(cancellationToken);

    public Task<UserAuthDetailsReadModel> LoadUserAuthDetailsReadModelAsync(UserId userId, CancellationToken cancellationToken = default) =>
        (
            from user in users
            where user.Id == userId
            select new UserAuthDetailsReadModel(
                user.Id,
                user.EmailAddress,
                user.PasswordHash,
                user.FirstName,
                user.LastName)
        )
        .SingleAsync(cancellationToken);

    public Task<UserAuthDetailsReadModel?> LoadUserAuthDetailsReadModelAsync(EmailAddress emailAddress, CancellationToken cancellationToken = default) =>
        (
            from user in users
            where user.EmailAddress == emailAddress
            select new UserAuthDetailsReadModel(
                user.Id,
                user.EmailAddress,
                user.PasswordHash,
                user.FirstName,
                user.LastName)
        )
        .SingleOrDefaultAsync(cancellationToken);

    public Task<UserDetailsReadModel> LoadUserDetailsReadModelAsync(UserId userId, CancellationToken cancellationToken = default) =>
        (
            from user in users
            join feeds in (
                    from feed in feeds
                    group feed by feed.UserId
                    into grouping
                    select new { UserId = grouping.Key, Count = grouping.Count() })
                on user.Id equals feeds.UserId
            join tags in (
                    from tag in tags
                    group tag by tag.UserId
                    into grouping
                    select new { UserId = grouping.Key, Count = grouping.Count() })
                on user.Id equals tags.UserId
            where user.Id == userId
            select new UserDetailsReadModel(
                user.EmailAddress,
                user.FirstName,
                user.LastName,
                user.Status == UserStatus.Active,
                feeds.Count,
                tags.Count)
        )
        .SingleAsync(cancellationToken);
}