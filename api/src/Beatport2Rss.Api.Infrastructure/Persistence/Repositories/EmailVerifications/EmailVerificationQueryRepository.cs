using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailVerifications;
using Beatport2Rss.Api.Domain.EmailVerifications;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Repositories.EmailVerifications;

internal sealed class EmailVerificationQueryRepository(ApiDbContext dbContext) :
    IEmailVerificationQueryRepository
{
    public async ValueTask<EmailVerification?> FindAsync(
        SecretHash secretHash,
        CancellationToken cancellationToken = default)
    {
        var match = await (
            from verification in dbContext.EmailVerifications.AsNoTracking()
            join user in dbContext.Users.AsNoTracking() on verification.UserId equals user.Id
            where verification.SecretHash == secretHash
            select new { Verification = verification, User = user })
            .SingleOrDefaultAsync(cancellationToken);

        return match is not null && match.Verification.AppliesTo(match.User) ? match.Verification : null;
    }
}
