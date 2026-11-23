using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.ReadModels.EmailConfirmations;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Repositories;

internal sealed class EmailConfirmationQueryRepository(
    IQueryable<EmailConfirmationQueryModel> emailConfirmationQueryModels) :
    QueryRepository<EmailConfirmationQueryModel, EmailConfirmationId>(emailConfirmationQueryModels),
    IEmailConfirmationQueryRepository
{
    public Task<EmailConfirmationReadModel?> FindAsync(
        EmailConfirmationId id,
        CancellationToken cancellationToken = default) =>
        FindAsync(
            confirmation => confirmation.Id == id,
            confirmation => new EmailConfirmationReadModel(
                confirmation.Id,
                confirmation.CreatedAt,
                confirmation.EmailAddress,
                confirmation.ExpiresAt,
                confirmation.ConfirmedAt),
            cancellationToken);
}
