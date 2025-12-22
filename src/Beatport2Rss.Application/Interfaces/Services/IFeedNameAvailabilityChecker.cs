namespace Beatport2Rss.Application.Interfaces.Services;

public interface IFeedNameAvailabilityChecker
{
    Task<bool> IsAvailableAsync(Guid userId, string name, CancellationToken cancellationToken = default);
}