namespace Beatport2Rss.Application.Interfaces.Services;

public interface IDatabaseHealthService
{
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);
}