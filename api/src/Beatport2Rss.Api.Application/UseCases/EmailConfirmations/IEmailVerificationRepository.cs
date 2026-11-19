using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations;

public interface IEmailVerificationRepository
{
    ValueTask AddAsync(EmailVerification verification, CancellationToken cancellationToken);
    ValueTask<EmailVerification?> FindAsync(string hash, CancellationToken cancellationToken);
    ValueTask<bool> ConfirmAsync(string hash, IClock clock, CancellationToken cancellationToken);
}
