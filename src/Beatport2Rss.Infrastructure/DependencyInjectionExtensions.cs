#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Infrastructure.Persistence;
using Beatport2Rss.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Slugify;

namespace Beatport2Rss.Infrastructure;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration) =>
            services
                .AddPersistence(configuration)
                .AddTransient<IEmailAddressAvailabilityChecker, UserService>()
                .AddTransient<IFeedNameAvailabilityChecker, FeedNameAvailabilityChecker>()
                .AddSingleton<IPasswordHasher, BCryptPasswordHasher>()
                .AddSingleton<ISlugGenerator, SlugGenerator>()
                .AddSingleton<ISlugHelper, SlugHelper>()
                .AddTransient<IUserExistenceChecker, UserService>();

        private IServiceCollection AddPersistence(IConfiguration configuration) =>
            services
                .AddDbContext<Beatport2RssDbContext>(b => b.UseNpgsql(configuration.GetConnectionString(nameof(Beatport2RssDbContext))))
                .AddTransient<IUnitOfWork, UnitOfWork>()
                .AddTransient<IUserCommandRepository, UserCommandRepository>()
                .AddTransient<IUserQueryRepository, UserQueryRepository>();
    }
}