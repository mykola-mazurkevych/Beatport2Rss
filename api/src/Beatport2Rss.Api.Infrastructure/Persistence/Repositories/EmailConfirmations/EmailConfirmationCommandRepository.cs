using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailConfirmations;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Repositories.EmailConfirmations;

internal sealed class EmailConfirmationCommandRepository(ApiDbContext dbContext) :
    CommandRepository<EmailConfirmation, EmailConfirmationId>(dbContext.EmailConfirmations),
    IEmailConfirmationCommandRepository
{
    async Task IEmailConfirmationCommandRepository.AddAsync(
        EmailConfirmation confirmation,
        CancellationToken cancellationToken) =>
        await AddAsync(confirmation, cancellationToken);

    public async Task<bool> ConfirmAsync(
        SecretHash secretHash,
        IClock clock,
        CancellationToken cancellationToken)
    {
        var candidate = await dbContext.EmailConfirmations.AsNoTracking()
            .SingleOrDefaultAsync(confirmation => confirmation.SecretHash == secretHash, cancellationToken);

        if (candidate is null)
        {
            return false;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        // Lock the user row before rereading the confirmation. This serializes confirmations
        // and prevents concurrent email changes or deletion while the confirmation commits.
        var users = await dbContext.Users
            .FromSql($"""SELECT * FROM api."Users" WHERE "Id" = {candidate.UserId.Value} FOR UPDATE""")
            .ToListAsync(cancellationToken);

        if (users.SingleOrDefault() is not { } user)
        {
            return false;
        }

        var confirmation = await dbContext.EmailConfirmations
            .SingleOrDefaultAsync(item => item.Id == candidate.Id, cancellationToken);

        if (confirmation is null || !confirmation.TryConfirm(user, clock.UtcNow))
        {
            return false;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }
}
