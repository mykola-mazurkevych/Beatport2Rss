namespace Beatport2Rss.Application.Interfaces.Services;

public interface IUsernameAvailabilityChecker
{
    Task<bool> IsAvailableAsync(string username, CancellationToken cancellationToken = default);
}