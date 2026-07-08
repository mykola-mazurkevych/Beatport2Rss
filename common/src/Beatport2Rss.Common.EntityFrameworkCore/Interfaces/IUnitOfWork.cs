namespace Beatport2Rss.Common.EntityFrameworkCore.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}