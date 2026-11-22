using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;

public interface IEmailVerificationCommandRepository
{
    Task AddAsync(EmailVerification verification, CancellationToken cancellationToken = default);
    Task<bool> ConfirmAsync(EmailVerificationSecretHash secretHash, IClock clock, CancellationToken cancellationToken = default);
}
