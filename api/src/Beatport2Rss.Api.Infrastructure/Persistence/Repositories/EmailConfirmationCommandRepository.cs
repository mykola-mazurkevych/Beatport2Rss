using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Repositories;

internal sealed class EmailConfirmationCommandRepository(ApiDbContext dbContext) :
    CommandRepository<EmailConfirmation, EmailConfirmationId>(dbContext.EmailConfirmations),
    IEmailConfirmationCommandRepository
{
    async Task IEmailConfirmationCommandRepository.AddAsync(
        EmailConfirmation confirmation,
        CancellationToken cancellationToken) =>
        await AddAsync(confirmation, cancellationToken);

}