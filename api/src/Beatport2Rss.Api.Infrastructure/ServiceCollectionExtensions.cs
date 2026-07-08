#pragma warning disable CA1034 // Nested types should not be visible

using System.Text;
using System.Text.Json;

using Beatport2Rss.Api.Application.Interfaces.Persistence;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Querying.Paging;
using Beatport2Rss.Api.Application.Interfaces.Services.Misc;
using Beatport2Rss.Api.Application.Interfaces.Services.Security;
using Beatport2Rss.Api.Domain.Countries;
using Beatport2Rss.Api.Infrastructure.Constants;
using Beatport2Rss.Api.Infrastructure.Extensions;
using Beatport2Rss.Api.Infrastructure.Options;
using Beatport2Rss.Api.Infrastructure.Persistence;
using Beatport2Rss.Api.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Api.Infrastructure.Persistence.Seeders;
using Beatport2Rss.Api.Infrastructure.Services.Health;
using Beatport2Rss.Api.Infrastructure.Services.Misc;
using Beatport2Rss.Api.Infrastructure.Services.Querying.Paging;
using Beatport2Rss.Api.Infrastructure.Services.Security;
using Beatport2Rss.Common.Beatport;
using Beatport2Rss.Common.BeatportTokenProvider;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Slugify;

namespace Beatport2Rss.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration) =>
            services
                .ConfigureHttpJsonOptions(static options => options.SerializerOptions.Configure())
                .ConfigureOptions(configuration)
                .AddBeatportServices(configuration)
                .AddHealthServices()
                .AddHttpClient()
                .AddJwtAuthentication(configuration.GetRequiredSection(nameof(JwtOptions)).Get<JwtOptions>()!)
                .AddMiscServices()
                .AddPaging()
                .AddPersistence(configuration)
                .AddSecurityServices();

        public IServiceCollection AddMigrator(IConfiguration configuration) =>
            services
                .AddDbContext(configuration)
                .AddTransient(provider => provider.GetRequiredService<ApiDbContext>().GetService<IMigrator>());

        private IServiceCollection AddBeatportServices(IConfiguration configuration) =>
            services
                .AddBeatportTokenProvider(configuration)
                .AddBeatportClient(configuration);

        private IServiceCollection AddDbContext(IConfiguration configuration) =>
            services
                .AddDbContext<ApiDbContext>(builder => builder
                    .UseNpgsql(
                        configuration.GetConnectionString(nameof(ApiDbContext)),
                        options => options.MigrationsHistoryTable("__EFMigrationsHistory", ApiDbContext.Schema))
                    .UseSeeding((dbContext, _) =>
                    {
                        CountriesSeeder.Seed(dbContext.Set<Country>());
                        dbContext.SaveChanges();
                    })
                    .UseAsyncSeeding(async (dbContext, _, cancellationToken) =>
                    {
                        await CountriesSeeder.SeedAsync(dbContext.Set<Country>(), cancellationToken);
                        await dbContext.SaveChangesAsync(cancellationToken);
                    }));

        private IServiceCollection AddHealthServices()
        {
            services
                .AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>(HealthCheckNames.Database);

            return services;
        }

        private IServiceCollection AddJwtAuthentication(JwtOptions jwtOptions)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Events.OnAuthenticationFailed = JwtEvents.OnAuthenticationFailed;
                    options.Events.OnTokenValidated = JwtEvents.OnTokenValidated;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                    };
                });

            services.AddAuthorization();

            return services;
        }

        private IServiceCollection AddMiscServices() =>
            services
                .AddSingleton<IClock, Clock>()
                .AddSingleton<ISlugGenerator, SlugGenerator>()
                .AddSingleton<ISlugHelper, SlugHelper>();

        private IServiceCollection AddPaging() =>
            services
                .AddSingleton<ICursorEncoder, CursorEncoder>()
                .AddSingleton<IPageBuilder, PageBuilder>();

        private IServiceCollection AddPersistence(IConfiguration configuration) =>
            services
                .AddDbContext(configuration)
                .AddTransient(provider => provider.GetRequiredService<ApiDbContext>().FeedQueryModels.AsNoTracking())
                .AddTransient(provider => provider.GetRequiredService<ApiDbContext>().Feeds)
                .AddTransient(provider => provider.GetRequiredService<ApiDbContext>().SessionQueryModels.AsNoTracking())
                .AddTransient(provider => provider.GetRequiredService<ApiDbContext>().Sessions)
                .AddTransient(provider => provider.GetRequiredService<ApiDbContext>().SubscriptionQueryModels.AsNoTracking())
                .AddTransient(provider => provider.GetRequiredService<ApiDbContext>().Subscriptions)
                .AddTransient(provider => provider.GetRequiredService<ApiDbContext>().SubscriptionTagQueryModels.AsNoTracking())
                .AddTransient(provider => provider.GetRequiredService<ApiDbContext>().TagQueryModels.AsNoTracking())
                .AddTransient(provider => provider.GetRequiredService<ApiDbContext>().Tags)
                .AddTransient(provider => provider.GetRequiredService<ApiDbContext>().UserQueryModels.AsNoTracking())
                .AddTransient(provider => provider.GetRequiredService<ApiDbContext>().Users)
                .AddTransient<IUnitOfWork, UnitOfWork>()
                .AddTransient<IFeedCommandRepository, FeedCommandRepository>()
                .AddTransient<IFeedQueryRepository, FeedQueryRepository>()
                .AddTransient<ISessionCommandRepository, SessionCommandRepository>()
                .AddTransient<ISessionQueryRepository, SessionQueryRepository>()
                .AddTransient<ISubscriptionCommandRepository, SubscriptionCommandRepository>()
                .AddTransient<ISubscriptionQueryRepository, SubscriptionQueryRepository>()
                .AddTransient<ITagCommandRepository, TagCommandRepository>()
                .AddTransient<ITagQueryRepository, TagQueryRepository>()
                .AddTransient<IUserCommandRepository, UserCommandRepository>()
                .AddTransient<IUserQueryRepository, UserQueryRepository>();

        private IServiceCollection AddSecurityServices() =>
            services
                .AddSingleton<IAccessTokenService, JwtService>()
                .AddSingleton<IPasswordHasher, BCryptPasswordHasher>()
                .AddSingleton<IRefreshTokenService, RefreshTokenService>();

        private IServiceCollection ConfigureOptions(IConfiguration configuration) =>
            services
                .Configure<BeatportCredentials>(credentials => configuration.GetSection(nameof(BeatportCredentials)).Bind(credentials))
                .Configure<JsonSerializerOptions>(static options => options.Configure())
                .Configure<JwtOptions>(options => configuration.GetSection(nameof(JwtOptions)).Bind(options))
                .Configure<RefreshTokenOptions>(options => configuration.GetSection(nameof(RefreshTokenOptions)).Bind(options));
    }
}