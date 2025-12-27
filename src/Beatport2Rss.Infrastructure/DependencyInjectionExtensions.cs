#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1708 // Identifiers should differ by more than case

using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Infrastructure.Persistence;
using Beatport2Rss.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Infrastructure.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Slugify;

using Wolverine;
using Wolverine.FluentValidation;

namespace Beatport2Rss.Infrastructure;

public static class DependencyInjectionExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public void AddInfrastructure()
        {
            builder.Host.UseWolverine(w =>
            {
                w.UseFluentValidation();
                w.Discovery.IncludeAssembly(typeof(IUnitOfWork).Assembly);
            });
            builder.Services.AddServices(builder.Configuration);
        }
    }

    extension(IServiceCollection services)
    {
        private void AddServices(IConfiguration configuration) =>
            services
                .AddPersistence(configuration)
                .AddTransient<IClock, Clock>()
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