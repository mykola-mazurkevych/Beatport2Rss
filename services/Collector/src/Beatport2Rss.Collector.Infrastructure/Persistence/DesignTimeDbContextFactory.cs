// ReSharper disable UnusedMember.Global

using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Beatport2Rss.Collector.Infrastructure.Persistence;

internal sealed class DesignTimeDbContextFactory :
    IDesignTimeDbContextFactory<ReleaseCollectorDbContext>
{
    public ReleaseCollectorDbContext CreateDbContext(string[] args)
    {
        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
            .Build();

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<ReleaseCollectorDbContext>();
        var connectionString = configurationRoot.GetConnectionString(nameof(ReleaseCollectorDbContext));

        dbContextOptionsBuilder.UseNpgsql(connectionString);

        return new ReleaseCollectorDbContext(dbContextOptionsBuilder.Options);
    }
}