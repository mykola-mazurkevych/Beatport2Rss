using Beatport2Rss.Common.Email.Interfaces;
using Beatport2Rss.Common.Email.Options;
using Beatport2Rss.Common.Email.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Resend;

namespace Beatport2Rss.Common.Email;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddEmailClient(IConfiguration configuration) =>
            services
                .ConfigureOptions(configuration)
                .AddResend()
                .AddTransient<IEmailClient, ResendEmailClient>();
        
        private IServiceCollection AddResend()
        {
            services
                .AddHttpClient<IResend, ResendClient>()
                .ConfigurePrimaryHttpMessageHandler(static () => new HttpClientHandler { AllowAutoRedirect = false })
                .RemoveAllLoggers();

            return services;
        }

        private IServiceCollection ConfigureOptions(IConfiguration configuration)
        {
            services
                .Configure<ResendOptions>(options => configuration.GetRequiredSection(nameof(ResendOptions)).Bind(options));

            services
                .AddOptions<ResendClientOptions>()
                .Configure<IOptions<ResendOptions>>(static (resendClientOptions, resendOptions) => resendClientOptions.ApiToken = resendOptions.Value.ApiToken);

            return services;
        }
    }
}