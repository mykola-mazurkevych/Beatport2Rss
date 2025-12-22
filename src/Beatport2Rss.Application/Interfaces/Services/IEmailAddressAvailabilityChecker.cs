namespace Beatport2Rss.Application.Interfaces.Services;

public interface IEmailAddressAvailabilityChecker
{
    Task<bool> IsAvailableAsync(string emailAddress, CancellationToken cancellationToken = default);
}