namespace Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}