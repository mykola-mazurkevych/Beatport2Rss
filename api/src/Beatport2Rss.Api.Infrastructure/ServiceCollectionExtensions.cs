#pragma warning disable CA1034 // Nested types should not be visible

using System.Net.Mail;
using System.Text;
using System.Text.Json;

using Beatport2Rss.Api.Application;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Querying.Paging;
using Beatport2Rss.Api.Application.Interfaces.Services.Feeds;
using Beatport2Rss.Api.Application.Interfaces.Services.Misc;
using Beatport2Rss.Api.Application.Interfaces.Services.Security;
using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Domain.Countries;
using Beatport2Rss.Api.Infrastructure.Constants;
using Beatport2Rss.Api.Infrastructure.Extensions;
using Beatport2Rss.Api.Infrastructure.Options;
using Beatport2Rss.Api.Infrastructure.Persistence;
using Beatport2Rss.Api.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Api.Infrastructure.Persistence.Seeders;
using Beatport2Rss.Api.Infrastructure.Services.Feeds;
using Beatport2Rss.Api.Infrastructure.Services.Health;
using Beatport2Rss.Api.Infrastructure.Services.Misc;
using Beatport2Rss.Api.Infrastructure.Services.Querying.Paging;
using Beatport2Rss.Api.Infrastructure.Services.Security;
using Beatport2Rss.Common.Beatport;
using Beatport2Rss.Common.BeatportTokenProvider;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;
using Beatport2Rss.Common.Grpc;
using Beatport2Rss.Common.IntegrationEvents.Feeds;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;
using Beatport2Rss.Common.IntegrationEvents.Tags;
using Beatport2Rss.Common.IntegrationEvents.Users;
using Beatport2Rss.Common.Miscellaneous;
using Beatport2Rss.Common.Wolverine;
using Beatport2Rss.Common.Wolverine.Extensions;

using RssFeedService;

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
                .AddConfirmationEmailOptions(configuration)
                .AddConfirmationEmailDelivery()
                .AddBeatportServices(configuration)
                .AddHealthServices()
                .AddHttpClient()
                .AddRssFeedDocumentClient(configuration)
                .AddJwtAuthentication(configuration.GetRequiredSection(nameof(JwtOptions)).Get<JwtOptions>()!)
                .AddMiscellaneous()
                .AddServices()
                .AddPaging()
                .AddPersistence(configuration)
                .AddWolverine<ApiDbContext, ApiApplication>(
                    configuration,
                    configurePublisher: options => options
                        .Publishes<FeedCreated>()
                        .Publishes<FeedDeleted>()
                        .Publishes<FeedSubscriptionCreated>()
                        .Publishes<FeedSubscriptionDeleted>()
                        .Publishes<FeedUpdated>()
                        .Publishes<SubscriptionRequested>()
                        .Publishes<SubscriptionTagCreated>()
                        .Publishes<SubscriptionTagDeleted>()
                        .Publishes<TagCreated>()
                        .Publishes<TagDeleted>()
                        .Publishes<TagUpdated>()
                        .Publishes<UserCreated>()
                        .Publishes<UserDeleted>(),
                    configureListener: listener => listener
                        .ListensTo<SubscriptionCreated>(),
                    configureRetries: options => options.ConfigureConfirmationEmailDelivery())
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
                        ApiDbContext.Schema)
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

        private IServiceCollection AddRssFeedDocumentClient(IConfiguration configuration) =>
            services
                .AddGrpcClient<GrpcRssFeedService.GrpcRssFeedServiceClient, RssFeedDocumentGrpcClient>(configuration)
                .AddTransient<IRssFeedDocumentClient, RssFeedDocumentGrpcClient>();

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

        private IServiceCollection AddServices() =>
            services
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
                .AddTransient<IFeedCommandRepository, FeedCommandRepository>()
                .AddTransient<IFeedQueryRepository, FeedQueryRepository>()
                .AddTransient<ISessionCommandRepository, SessionCommandRepository>()
                .AddTransient<ISessionQueryRepository, SessionQueryRepository>()
                .AddTransient<ISubscriptionCommandRepository, SubscriptionCommandRepository>()
                .AddTransient<ISubscriptionQueryRepository, SubscriptionQueryRepository>()
                .AddTransient<ITagCommandRepository, TagCommandRepository>()
                .AddTransient<ITagQueryRepository, TagQueryRepository>()
                .AddTransient<IUserCommandRepository, UserCommandRepository>()
                .AddTransient<IEmailVerificationRepository, EmailVerificationRepository>()
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

        private IServiceCollection AddConfirmationEmailOptions(IConfiguration configuration)
        {
            services.AddOptions<EmailConfirmationOptions>()
                .Bind(configuration.GetSection(EmailConfirmationOptions.SectionName))
                .Validate(options => options.Lifetime > TimeSpan.Zero && options.Lifetime <= TimeSpan.FromDays(365),
                    "EmailConfirmation:Lifetime must be positive and no longer than 365 days.")
                .Validate(options => options.AdministratorUserId != Guid.Empty,
                    "EmailConfirmation:AdministratorUserId must not be the empty GUID.")
                .Validate(options => !options.Enabled || !string.IsNullOrWhiteSpace(options.ApiKey),
                    "EmailConfirmation:ApiKey is required when confirmation emails are enabled.")
                .Validate(options => !options.Enabled ||
                    (MailAddress.TryCreate(options.SenderAddress, out var address) &&
                     address.Address == options.SenderAddress &&
                     !string.IsNullOrWhiteSpace(options.SenderName) &&
                     !options.SenderName.Any(char.IsControl)),
                    "EmailConfirmation requires a valid SenderAddress and SenderName when enabled.")
                .Validate(options => !options.Enabled ||
                    (options.PublicWebBaseUrl.IsAbsoluteUri &&
                     options.PublicWebBaseUrl.Scheme is "http" or "https" &&
                     options.PublicWebBaseUrl.Host.Length > 0 &&
                     options.PublicWebBaseUrl.UserInfo.Length == 0 &&
                     options.PublicWebBaseUrl.Query.Length == 0 &&
                     options.PublicWebBaseUrl.Fragment.Length == 0),
                    "EmailConfirmation:PublicWebBaseUrl must be an absolute HTTP(S) URL without credentials, query, or fragment.")
                .ValidateOnStart();

            return services;
        }
    }
}