using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailVerifications;
using Beatport2Rss.Api.Domain.EmailVerifications;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Repositories.EmailVerifications;

internal sealed class EmailVerificationCommandRepository(ApiDbContext dbContext) :
    CommandRepository<EmailVerification, EmailVerificationId>(dbContext.EmailVerifications),
    IEmailVerificationCommandRepository
{
    async Task IEmailVerificationCommandRepository.AddAsync(
        EmailVerification verification,
        CancellationToken cancellationToken) =>
        await AddAsync(verification, cancellationToken);

    public async Task<bool> ConfirmAsync(
        EmailVerificationSecretHash secretHash,
        IClock clock,
        CancellationToken cancellationToken)
    {
        var candidate = await dbContext.EmailVerifications.AsNoTracking()
            .SingleOrDefaultAsync(verification => verification.SecretHash == secretHash, cancellationToken);

        if (candidate is null)
        {
            return false;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        // Lock the user row before rereading the verification. This serializes confirmations
        // and prevents concurrent email changes or deletion while the confirmation commits.
        var users = await dbContext.Users
            .FromSql($"""SELECT * FROM api."Users" WHERE "Id" = {candidate.UserId.Value} FOR UPDATE""")
            .ToListAsync(cancellationToken);

        if (users.SingleOrDefault() is not { } user)
        {
            return false;
        }

        var verification = await dbContext.EmailVerifications
            .SingleOrDefaultAsync(item => item.Id == candidate.Id, cancellationToken);

        if (verification is null || !verification.TryConfirm(user, clock.UtcNow))
        {
            return false;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }
}
