using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Beatport2Rss.Infrastructure.Persistence;

internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<Beatport2RssDbContext>
{
    public Beatport2RssDbContext CreateDbContext(string[] args)
    {
        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
            .Build();

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<Beatport2RssDbContext>();
        var connectionString = configurationRoot.GetConnectionString(nameof(Beatport2RssDbContext));

        dbContextOptionsBuilder.UseSqlServer(connectionString);

        return new Beatport2RssDbContext(dbContextOptionsBuilder.Options);
    }
}