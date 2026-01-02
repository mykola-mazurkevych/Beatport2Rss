using Beatport2Rss.Application.Interfaces.Services.Checkers;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Services.Checkers;

internal sealed class UserChecker(Beatport2RssDbContext dbContext) :
    IEmailAddressAvailabilityChecker,
    IUserExistenceChecker
{
    Task<bool> IEmailAddressAvailabilityChecker.IsAvailableAsync(EmailAddress emailAddress, CancellationToken cancellationToken) =>
        dbContext.Users.AllAsync(u => u.EmailAddress != emailAddress, cancellationToken);

    Task<bool> IUserExistenceChecker.ExistsAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.Users.AnyAsync(u => u.Id == userId, cancellationToken);
}