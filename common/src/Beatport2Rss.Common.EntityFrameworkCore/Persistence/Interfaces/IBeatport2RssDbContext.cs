namespace Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

public interface IBeatport2RssDbContext
{
    static abstract string Schema { get; }
}