using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailConfirmations;
using Beatport2Rss.Api.Domain.EmailConfirmations;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Repositories.EmailConfirmations;

internal sealed class EmailConfirmationQueryRepository(ApiDbContext dbContext) :
    IEmailConfirmationQueryRepository
{
    public async ValueTask<EmailConfirmation?> FindAsync(
        SecretHash secretHash,
        CancellationToken cancellationToken = default)
    {
        var match = await (
            from confirmation in dbContext.EmailConfirmations.AsNoTracking()
            join user in dbContext.Users.AsNoTracking() on confirmation.UserId equals user.Id
            where confirmation.SecretHash == secretHash
            select new { Confirmation = confirmation, User = user })
            .SingleOrDefaultAsync(cancellationToken);

        return match is not null && match.Confirmation.AppliesTo(match.User) ? match.Confirmation : null;
    }
}
