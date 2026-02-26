#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1708 // Identifiers should differ by more than case

using System.Text;
using System.Text.Json;

using Beatport2Rss.Application.Interfaces.Pagination;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Beatport;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Application.Interfaces.Services.Security;
using Beatport2Rss.Infrastructure.Constants;
using Beatport2Rss.Infrastructure.Extensions;
using Beatport2Rss.Infrastructure.Options;
using Beatport2Rss.Infrastructure.Persistence;
using Beatport2Rss.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Infrastructure.Services.Beatport;
using Beatport2Rss.Infrastructure.Services.Health;
using Beatport2Rss.Infrastructure.Services.Misc;
using Beatport2Rss.Infrastructure.Services.Pagination;
using Beatport2Rss.Infrastructure.Services.Security;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Slugify;

namespace Beatport2Rss.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration) =>
            services
                .ConfigureHttpJsonOptions(static options => options.SerializerOptions.Configure())
                .ConfigureOptions(configuration)
                .AddBeatportServices()
                .AddHealthServices()
                .AddJwtAuthentication(configuration.GetRequiredSection(nameof(JwtOptions)).Get<JwtOptions>()!)
                .AddMiscServices()
                .AddPagination()
                .AddPersistence(configuration)
                .AddSecurityServices();

        private IServiceCollection AddBeatportServices() =>
            services
                .AddSingleton<IBeatportAccessTokenProvider, BeatportAccessTokenProvider>();

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
                .AddSingleton<IChromiumDownloader, ChromiumDownloader>()
                .AddSingleton<IClock, Clock>()
                .AddSingleton<ISlugGenerator, SlugGenerator>()
                .AddSingleton<ISlugHelper, SlugHelper>();

        private IServiceCollection AddPagination() =>
            services
                .AddSingleton<ICursorEncoder, CursorEncoder>()
                .AddSingleton<IPageBuilder, PageBuilder>();

        private IServiceCollection AddPersistence(IConfiguration configuration) =>
            services
                .AddDbContext<Beatport2RssDbContext>(builder => builder.UseNpgsql(configuration.GetConnectionString(nameof(Beatport2RssDbContext))))
                .AddTransient<IUnitOfWork, UnitOfWork>()
                .AddTransient<IFeedQueryRepository, FeedQueryRepository>()
                .AddTransient<ISessionCommandRepository, SessionCommandRepository>()
                .AddTransient<ISessionQueryRepository, SessionQueryRepository>()
                .AddTransient<ITagQueryRepository, TagQueryRepository>()
                .AddTransient<ITokenCommandRepository, TokenCommandRepository>()
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