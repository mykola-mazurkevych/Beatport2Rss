#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Common.Grpc.Interfaces;
using Beatport2Rss.Common.Grpc.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Beatport2Rss.Common.Grpc;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddGrpcClient<TGrpcClient, TGrpcClientService>(IConfiguration configuration)
            where TGrpcClient : class
            where TGrpcClientService : IBeatport2RssGrpcClient
        {
            var section = configuration.GetRequiredSection($"{nameof(GrpcOptions)}:{TGrpcClientService.OptionName}");
            services
                .AddOptions<GrpcOptions>(TGrpcClientService.OptionName)
                .Configure(section.Bind)
                .Validate(
                    options => options.Address is { IsAbsoluteUri: true },
                    $"{nameof(GrpcOptions)}:{TGrpcClientService.OptionName}:{nameof(GrpcOptions.Address)} must be an absolute URI.")
                .ValidateOnStart();

            services
                .AddGrpcClient<TGrpcClient>((provider, clientOptions) => clientOptions.Address = provider.GetGrpcAddress<TGrpcClientService>())
                .ConfigurePrimaryHttpMessageHandler(provider =>
                {
                    if (provider.GetGrpcAddress<TGrpcClientService>().Scheme == Uri.UriSchemeHttp)
                    {
                        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                    }

                    return new SocketsHttpHandler { EnableMultipleHttp2Connections = true };
                });

            return services;
        }
    }

    extension(IServiceProvider serviceProvider)
    {
        private Uri GetGrpcAddress<TGrpcClientService>()
            where TGrpcClientService : IBeatport2RssGrpcClient =>
            serviceProvider.GetRequiredService<IOptionsMonitor<GrpcOptions>>().Get(TGrpcClientService.OptionName).Address;
    }
}