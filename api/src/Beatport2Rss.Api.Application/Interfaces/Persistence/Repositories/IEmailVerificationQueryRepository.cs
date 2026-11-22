using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;

public interface IEmailVerificationQueryRepository
{
    ValueTask<EmailVerification?> FindAsync(
        EmailVerificationSecretHash secretHash,
        CancellationToken cancellationToken = default);
}
