#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Beatport.Options;
using Beatport2Rss.Common.Beatport.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Beatport2Rss.Common.Beatport;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddBeatportClient(IConfiguration configuration)
        {
            services.Configure<BeatportOptions>(options => configuration.GetSection(nameof(BeatportOptions)).Bind(options));

            services.AddSingleton<IBeatportUriBuilder, BeatportUriBuilder>();
            services.AddHttpClient<IBeatportClient, BeatportClient>((provider, httpClient) =>
            {
                var options = provider.GetRequiredService<IOptions<BeatportOptions>>().Value;
                httpClient.BaseAddress = new Uri(options.ApiV4BaseUri.ToString().TrimEnd('/') + '/');
            });

            return services;
        }
    }
}