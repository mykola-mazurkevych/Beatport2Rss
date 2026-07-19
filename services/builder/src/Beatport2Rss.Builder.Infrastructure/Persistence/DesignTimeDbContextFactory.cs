// ReSharper disable UnusedMember.Global

using System.Reflection;

using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Beatport2Rss.Builder.Infrastructure.Persistence;

internal sealed class DesignTimeDbContextFactory :
    IDesignTimeDbContextFactory<BuilderDbContext>
{
    public BuilderDbContext CreateDbContext(string[] args)
    {
        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
            .Build();

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<BuilderDbContext>();
        var connectionString = configurationRoot.GetConnectionString(nameof(BuilderDbContext));
        dbContextOptionsBuilder.UseNpgsql(connectionString, BuilderDbContext.Schema);

        return new BuilderDbContext(dbContextOptionsBuilder.Options);
    }
}