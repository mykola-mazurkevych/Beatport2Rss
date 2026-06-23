#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Common.BeatportTokenProvider.Options;
using Beatport2Rss.Common.BeatportTokenProvider.Services.Interfaces;

using BeatportAccessTokenService;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Beatport2Rss.Common.BeatportTokenProvider;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddBeatportTokenProvider(IConfiguration configuration)
        {
            services.Configure<BeatportTokenProviderOptions>(credentials => configuration.GetSection(nameof(BeatportTokenProviderOptions)).Bind(credentials));

            services.AddGrpcClient<GrpcBeatportAccessTokenService.GrpcBeatportAccessTokenServiceClient>((provider, options) =>
            {
                var tokenOptions = provider.GetRequiredService<IOptions<BeatportTokenProviderOptions>>().Value;
                options.Address = tokenOptions.TokenInterceptorGrpcUri;
            });

            services.AddSingleton<IBeatportTokenProvider, Services.BeatportTokenProvider>();

            return services;
        }
    }
}