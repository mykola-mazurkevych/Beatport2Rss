namespace Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}