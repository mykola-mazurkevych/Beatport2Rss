#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1708 // Identifiers should differ by more than case

using System.Text;
using System.Text.Json.Serialization;

using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Application.Options;
using Beatport2Rss.Infrastructure.Persistence;
using Beatport2Rss.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Infrastructure.Services;

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
                .ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()))
                .ConfigureOptions(builder.Configuration)
                .AddJwtAuthentication(builder.Configuration.GetRequiredSection(nameof(JwtOptions)).Get<JwtOptions>()!)
                .AddServices(builder.Configuration);
        }
    }

    extension(IServiceCollection services)
    {
        private IServiceCollection ConfigureOptions(IConfiguration configuration) =>
            services
                .Configure<JwtOptions>(o => configuration.GetSection(nameof(JwtOptions)).Bind(o))
                .Configure<RefreshTokenOptions>(o => configuration.GetSection(nameof(RefreshTokenOptions)).Bind(o));

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

        private void AddServices(IConfiguration configuration) =>
            services
                .AddValidators()
                .AddPersistence(configuration)
                .AddSingleton<IAccessTokenService, JwtService>()
                .AddSingleton<IClock, Clock>()
                .AddTransient<IEmailAddressAvailabilityChecker, UserService>()
                .AddTransient<IFeedNameAvailabilityChecker, FeedNameAvailabilityChecker>()
                .AddSingleton<IPasswordHasher, BCryptPasswordHasher>()
                .AddSingleton<IRefreshTokenService, RefreshTokenService>()
                .AddSingleton<ISlugGenerator, SlugGenerator>()
                .AddSingleton<ISlugHelper, SlugHelper>()
                .AddTransient<IUserExistenceChecker, UserService>();

        private IServiceCollection AddValidators() =>
            services.Scan(s =>
            {
                s.Assembly(typeof(IUnitOfWork).Assembly);
                s.ConnectImplementationsToTypesClosing(typeof(IValidator<>), ServiceLifetime.Transient);
            });

        private IServiceCollection AddPersistence(IConfiguration configuration) =>
            services
                .AddDbContext<Beatport2RssDbContext>(b => b.UseNpgsql(configuration.GetConnectionString(nameof(Beatport2RssDbContext))))
                .AddTransient<IUnitOfWork, UnitOfWork>()
                .AddTransient<ISessionCommandRepository, SessionCommandRepository>()
                .AddTransient<ISessionQueryRepository, SessionQueryRepository>()
                .AddTransient<IUserCommandRepository, UserCommandRepository>()
                .AddTransient<IUserQueryRepository, UserQueryRepository>();
    }
}