using Beatport2Rss.Api.Application.ReadModels.EmailConfirmations;
using Beatport2Rss.Api.Domain.EmailConfirmations;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;

public interface IEmailConfirmationQueryRepository
{
    Task<EmailConfirmationReadModel?> FindAsync(EmailConfirmationId id, CancellationToken cancellationToken = default);
}