#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Collector.Infrastructure.Persistence;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Collector.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMigrator(IConfiguration configuration) =>
            services
                .AddDbContext(configuration)
                .AddTransient(provider => provider.GetRequiredService<CollectorDbContext>().GetService<IMigrator>());

        private IServiceCollection AddDbContext(IConfiguration configuration) =>
            services
                .AddDbContext<CollectorDbContext>(builder => builder
                    .UseNpgsql(
                        configuration.GetConnectionString(nameof(CollectorDbContext)),
                        CollectorDbContext.Schema));
    }
}