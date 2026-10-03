#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Infrastructure.Persistence;
using Beatport2Rss.Builder.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Common.EntityFrameworkCore;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;
using Beatport2Rss.Common.Messaging;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Builder.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration) =>
            services
                .AddPersistence(configuration);

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

        private IServiceCollection AddPersistence(IConfiguration configuration) =>
            services
                .AddDbContext(configuration)
                .AddInboxPersistence<BuilderDbContext>()
                .AddOutboxPersistence<BuilderDbContext>()
                .AddUnitOfWork<BuilderDbContext>()
                .AddTransient(provider => provider.GetRequiredService<BuilderDbContext>().Releases)
                .AddTransient<IReleaseCommandRepository, ReleaseCommandRepository>();
    }
}