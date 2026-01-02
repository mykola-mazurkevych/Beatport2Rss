#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1708 // Identifiers should differ by more than case

using System.Text.Json;
using System.Text.Json.Serialization;

using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Checkers;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Application.Interfaces.Services.Security;
using Beatport2Rss.Infrastructure.Constants;
using Beatport2Rss.Infrastructure.Persistence;
using Beatport2Rss.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Infrastructure.Services.Checkers;
using Beatport2Rss.Infrastructure.Services.Health;
using Beatport2Rss.Infrastructure.Services.Misc;
using Beatport2Rss.Infrastructure.Services.Security;

using FluentValidation;

using JasperFx.Core.IoC;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Slugify;

using Wolverine;

namespace Beatport2Rss.Infrastructure;

public static class DependencyInjectionExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public void AddInfrastructure()
        {
            builder.Host.UseWolverine(w => w.Discovery.IncludeAssembly(typeof(IUnitOfWork).Assembly));

            builder.Services.ConfigureHttpJsonOptions(o =>
            {
                o.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                o.SerializerOptions.WriteIndented = true;
                o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            builder.Services.AddServices(builder.Configuration);
        }
    }

    extension(IServiceCollection services)
    {
        private void AddServices(IConfiguration configuration) =>
            services
                .AddPersistence(configuration)
                .AddCheckers()
                .AddHealthServices()
                .AddMiscServices()
                .AddSecurityServices()
                .AddValidators();

        private IServiceCollection AddCheckers() =>
            services
                .AddTransient<IEmailAddressAvailabilityChecker, UserChecker>()
                .AddTransient<IFeedNameAvailabilityChecker, FeedNameAvailabilityChecker>()
                .AddTransient<IUserExistenceChecker, UserChecker>();

        private IServiceCollection AddHealthServices()
        {
            services
                .AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>(HealthCheckNames.Database);

            return services;
        }

        private IServiceCollection AddMiscServices() =>
            services
                .AddSingleton<IClock, Clock>()
                .AddSingleton<ISlugGenerator, SlugGenerator>()
                .AddSingleton<ISlugHelper, SlugHelper>();

        private IServiceCollection AddPersistence(IConfiguration configuration) =>
            services
                .AddDbContext<Beatport2RssDbContext>(b => b.UseNpgsql(configuration.GetConnectionString(nameof(Beatport2RssDbContext))))
                .AddTransient<IUnitOfWork, UnitOfWork>()
                .AddTransient<IUserCommandRepository, UserCommandRepository>()
                .AddTransient<IUserQueryRepository, UserQueryRepository>();

        private IServiceCollection AddSecurityServices() =>
            services
                .AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        private void AddValidators() =>
            services
                .Scan(s =>
                {
                    s.Assembly(typeof(IUnitOfWork).Assembly);
                    s.ConnectImplementationsToTypesClosing(typeof(IValidator<>), ServiceLifetime.Transient);
                });
    }
}