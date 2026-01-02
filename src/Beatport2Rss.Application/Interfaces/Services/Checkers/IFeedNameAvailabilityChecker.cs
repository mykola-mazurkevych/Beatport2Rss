namespace Beatport2Rss.Application.Interfaces.Services.Checkers;

public interface IFeedNameAvailabilityChecker
{
    Task<bool> IsAvailableAsync(Guid userId, string name, CancellationToken cancellationToken = default);
}