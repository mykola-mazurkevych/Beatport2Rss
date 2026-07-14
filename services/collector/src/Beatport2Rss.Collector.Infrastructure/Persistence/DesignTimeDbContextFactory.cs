// ReSharper disable UnusedMember.Global

using System.Reflection;

using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Beatport2Rss.Collector.Infrastructure.Persistence;

internal sealed class DesignTimeDbContextFactory :
    IDesignTimeDbContextFactory<CollectorDbContext>
{
    public CollectorDbContext CreateDbContext(string[] args)
    {
        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
            .Build();

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<CollectorDbContext>();
        var connectionString = configurationRoot.GetConnectionString(nameof(CollectorDbContext));
        dbContextOptionsBuilder.UseNpgsql(connectionString, CollectorDbContext.Schema);

        return new CollectorDbContext(dbContextOptionsBuilder.Options);
    }
}