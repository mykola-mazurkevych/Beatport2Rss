using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Repositories;

internal sealed class EmailVerificationRepository(ApiDbContext dbContext) : IEmailVerificationRepository
{
    public async ValueTask AddAsync(EmailVerification verification, CancellationToken cancellationToken) =>
        await dbContext.EmailVerifications.AddAsync(verification, cancellationToken);

    public async ValueTask<EmailVerification?> FindAsync(string hash, CancellationToken cancellationToken)
    {
        var match = await (
            from verification in dbContext.EmailVerifications.AsNoTracking()
            join user in dbContext.Users.AsNoTracking() on verification.UserId equals user.Id
            where verification.SecretHash == hash
            select new { Verification = verification, User = user })
            .SingleOrDefaultAsync(cancellationToken);

        return match is not null && match.Verification.AppliesTo(match.User) ? match.Verification : null;
    }

    public async ValueTask<bool> ConfirmAsync(string hash, IClock clock, CancellationToken cancellationToken)
    {
        var candidate = await dbContext.EmailVerifications.AsNoTracking()
            .SingleOrDefaultAsync(verification => verification.SecretHash == hash, cancellationToken);

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
