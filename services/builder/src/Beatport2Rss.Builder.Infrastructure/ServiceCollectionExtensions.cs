#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Builder.Infrastructure.Persistence;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Builder.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMigrator(IConfiguration configuration) =>
            services
                .AddDbContext(configuration)
                .AddTransient(provider => provider.GetRequiredService<BuilderDbContext>().GetService<IMigrator>());

        private IServiceCollection AddDbContext(IConfiguration configuration) =>
            services
                .AddDbContext<BuilderDbContext>(builder => builder
                    .UseNpgsql(
                        configuration.GetConnectionString(nameof(BuilderDbContext)),
                        BuilderDbContext.Schema));
    }
}