using Beatport2Rss.Api.Domain.EmailVerifications;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailVerifications;

public interface IEmailVerificationQueryRepository
{
    ValueTask<EmailVerification?> FindAsync(
        EmailVerificationSecretHash secretHash,
        CancellationToken cancellationToken = default);
}
