// ReSharper disable UnusedMember.Global

using System.Reflection;

using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Beatport2Rss.Api.Infrastructure.Persistence;

internal sealed class DesignTimeDbContextFactory :
    IDesignTimeDbContextFactory<ApiDbContext>
{
    public ApiDbContext CreateDbContext(string[] args)
    {
        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
            .Build();

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<ApiDbContext>();
        var connectionString = configurationRoot.GetConnectionString(nameof(ApiDbContext));
        dbContextOptionsBuilder.UseNpgsql(connectionString, ApiDbContext.Schema);

        return new ApiDbContext(dbContextOptionsBuilder.Options);
    }
}