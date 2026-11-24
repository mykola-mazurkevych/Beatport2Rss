using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Repositories;

internal sealed class EmailConfirmationCommandRepository(ApiDbContext dbContext) :
    CommandRepository<EmailConfirmation, EmailConfirmationId>(dbContext.EmailConfirmations),
    IEmailConfirmationCommandRepository
{
    public Task<UserId?> FindUserIdAsync(EmailConfirmationId id, CancellationToken cancellationToken = default) =>
        dbContext.EmailConfirmations
            .AsNoTracking()
            .Where(confirmation => confirmation.Id == id)
            .Select(confirmation => (UserId?)confirmation.UserId)
            .SingleOrDefaultAsync(cancellationToken);

    async Task IEmailConfirmationCommandRepository.AddAsync(
        EmailConfirmation confirmation,
        CancellationToken cancellationToken) =>
        await AddAsync(confirmation, cancellationToken);

}