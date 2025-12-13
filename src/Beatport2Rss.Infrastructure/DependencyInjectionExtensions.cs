#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Contracts.Interfaces;
using Beatport2Rss.Contracts.Persistence;
using Beatport2Rss.Contracts.Persistence.Repositories;
using Beatport2Rss.Infrastructure.Persistence;
using Beatport2Rss.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Infrastructure.Security;
using Beatport2Rss.Infrastructure.Utilities;

using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Infrastructure;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure() =>
            services
                .AddPersistence()
                .AddSingleton<IPasswordHasher, BCryptPasswordHasher>()
                .AddSingleton<ISlugGenerator, SlugGenerator>();

        private IServiceCollection AddPersistence() =>
            services
                .AddDbContext<Beatport2RssDbContext>()
                .AddTransient<IUnitOfWork, UnitOfWork>()
                .AddTransient<IUserCommandRepository, UserCommandRepository>()
                .AddTransient<IUserQueryRepository, UserQueryRepository>();
    }
}