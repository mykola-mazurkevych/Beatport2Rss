namespace Beatport2Rss.Application.Interfaces.Services;

public interface IUserExistenceChecker
{
    Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}