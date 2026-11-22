using Beatport2Rss.Api.Domain.EmailConfirmations;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailConfirmations;

public interface IEmailConfirmationQueryRepository
{
    ValueTask<EmailConfirmation?> FindAsync(
        SecretHash secretHash,
        CancellationToken cancellationToken = default);
}
