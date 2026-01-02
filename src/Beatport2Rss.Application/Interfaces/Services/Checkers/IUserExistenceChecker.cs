namespace Beatport2Rss.Application.Interfaces.Services.Checkers;

public interface IUserExistenceChecker
{
    Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}