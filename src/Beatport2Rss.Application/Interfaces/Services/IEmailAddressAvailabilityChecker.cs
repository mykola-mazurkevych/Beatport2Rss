using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Services;

public interface IEmailAddressAvailabilityChecker
{
    Task<bool> IsAvailableAsync(EmailAddress emailAddress, CancellationToken cancellationToken = default);
}