using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Services;

internal sealed class UserService(Beatport2RssDbContext dbContext) :
    IEmailAddressAvailabilityChecker,
    IUserExistenceChecker
{
    Task<bool> IEmailAddressAvailabilityChecker.IsAvailableAsync(string emailAddress, CancellationToken cancellationToken) =>
        dbContext.Users.AllAsync(u => u.EmailAddress != emailAddress, cancellationToken);

    Task<bool> IUserExistenceChecker.ExistsAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.Users.AnyAsync(u => u.Id == userId, cancellationToken);
}