using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Services;

internal sealed class UserService(Beatport2RssDbContext dbContext) :
    IEmailAddressAvailabilityChecker,
    IUsernameAvailabilityChecker,
    IUserExistenceChecker
{
    Task<bool> IUsernameAvailabilityChecker.IsAvailableAsync(string username, CancellationToken cancellationToken) =>
        dbContext.Users.AllAsync(u => u.Username != username, cancellationToken);

    Task<bool> IEmailAddressAvailabilityChecker.IsAvailableAsync(string emailAddress, CancellationToken cancellationToken) =>
        dbContext.Users.AllAsync(u => u.EmailAddress != emailAddress, cancellationToken);

    Task<bool> IUserExistenceChecker.ExistsAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.Users.AnyAsync(u => u.Id == userId, cancellationToken);
}