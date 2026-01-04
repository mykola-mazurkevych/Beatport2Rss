#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1708 // Identifiers should differ by more than case

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Application.Interfaces.Services.Checkers;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Application.Interfaces.Services.Security;
using Beatport2Rss.Application.Options;
using Beatport2Rss.Infrastructure.Constants;
using Beatport2Rss.Infrastructure.Persistence;
using Beatport2Rss.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Infrastructure.Services.Checkers;
using Beatport2Rss.Infrastructure.Services.Health;
using Beatport2Rss.Infrastructure.Services.Misc;
using Beatport2Rss.Infrastructure.Services.Security;

using FluentValidation;

using JasperFx.Core.IoC;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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

            builder.Services
                .ConfigureHttpJsonOptions(o =>
                {
                    o.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    o.SerializerOptions.WriteIndented = true;
                    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .ConfigureOptions(builder.Configuration)
                .AddCheckers()
                .AddHealthServices()
                .AddJwtAuthentication(builder.Configuration.GetRequiredSection(nameof(JwtOptions)).Get<JwtOptions>()!)
                .AddMiscServices()
                .AddPersistence(builder.Configuration)
                .AddSecurityServices()
                .AddValidators();
        }
    }

    extension(IServiceCollection services)
    {
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

        private IServiceCollection AddJwtAuthentication(JwtOptions jwtOptions)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.Events.OnAuthenticationFailed = JwtEvents.OnAuthenticationFailed;
                    o.Events.OnTokenValidated = JwtEvents.OnTokenValidated;

                    o.TokenValidationParameters = new TokenValidationParameters
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

        private IServiceCollection AddPersistence(IConfiguration configuration) =>
            services
                .AddDbContext<Beatport2RssDbContext>(b => b.UseNpgsql(configuration.GetConnectionString(nameof(Beatport2RssDbContext))))
                .AddTransient<IUnitOfWork, UnitOfWork>()
                .AddTransient<ISessionCommandRepository, SessionCommandRepository>()
                .AddTransient<ISessionQueryRepository, SessionQueryRepository>()
                .AddTransient<IUserCommandRepository, UserCommandRepository>()
                .AddTransient<IUserQueryRepository, UserQueryRepository>();

        private IServiceCollection AddSecurityServices() =>
            services
                .AddSingleton<IAccessTokenService, JwtService>()
                .AddSingleton<IPasswordHasher, BCryptPasswordHasher>()
                .AddSingleton<IRefreshTokenService, RefreshTokenService>();

        private void AddValidators() =>
            services
                .Scan(s =>
                {
                    s.Assembly(typeof(IUnitOfWork).Assembly);
                    s.ConnectImplementationsToTypesClosing(typeof(IValidator<>), ServiceLifetime.Transient);
                });

        private IServiceCollection ConfigureOptions(IConfiguration configuration) =>
            services
                .Configure<JwtOptions>(o => configuration.GetSection(nameof(JwtOptions)).Bind(o))
                .Configure<RefreshTokenOptions>(o => configuration.GetSection(nameof(RefreshTokenOptions)).Bind(o));
    }
}