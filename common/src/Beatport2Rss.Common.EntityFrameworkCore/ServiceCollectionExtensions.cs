#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Common.EntityFrameworkCore.Persistence;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Common.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddUnitOfWork<TDbContext>()
            where TDbContext : DbContext =>
            services.AddTransient<IUnitOfWork, UnitOfWork>(provider =>
            {
                var dbContext = provider.GetRequiredService<TDbContext>();
                return new UnitOfWork(dbContext);
            });
    }
}