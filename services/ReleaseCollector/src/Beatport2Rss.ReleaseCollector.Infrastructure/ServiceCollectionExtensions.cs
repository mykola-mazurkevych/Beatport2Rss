#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.ReleaseCollector.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.ReleaseCollector.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMigrator(IConfiguration configuration) =>
            services
                .AddDbContext(configuration)
                .AddTransient(provider => provider.GetRequiredService<ReleaseCollectorDbContext>().GetService<IMigrator>());

        private IServiceCollection AddDbContext(IConfiguration configuration) =>
            services
                .AddDbContext<ReleaseCollectorDbContext>(builder => builder
                    .UseNpgsql(configuration.GetConnectionString(nameof(ReleaseCollectorDbContext))));
    }
}