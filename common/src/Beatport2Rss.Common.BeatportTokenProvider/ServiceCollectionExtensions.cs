#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Grpc;

using BeatportAccessTokenService;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Common.BeatportTokenProvider;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddBeatportTokenProvider(IConfiguration configuration) =>
            services
                .AddGrpcClient<GrpcBeatportAccessTokenService.GrpcBeatportAccessTokenServiceClient, Services.BeatportTokenProvider>(configuration)
                .AddSingleton<IBeatportTokenProvider, Services.BeatportTokenProvider>();
    }
}