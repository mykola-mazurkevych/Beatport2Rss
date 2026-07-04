#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Common.Messaging.Interfaces;
using Beatport2Rss.Common.Messaging.Options;
using Beatport2Rss.Common.Messaging.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace Beatport2Rss.Common.Messaging;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMessaging(IConfiguration configuration) =>
            services
                .ConfigureOptions(configuration)
                .AddConnectionFactory()
                .AddSingleton<IPublisher, RabbitMqPublisher>();

        private IServiceCollection ConfigureOptions(IConfiguration configuration) =>
            services
                .Configure<RabbitMqOptions>(options => configuration.GetSection(nameof(RabbitMqOptions)).Bind(options))
                .Configure<QueueOptions>(options => configuration.GetSection(nameof(QueueOptions)).Bind(options));

        private IServiceCollection AddConnectionFactory() =>
            services.AddSingleton<IConnectionFactory>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                return new ConnectionFactory
                {
                    HostName = options.HostName,
                    Port = options.Port,
                    UserName = options.UserName,
                    Password = options.Password,
                    VirtualHost = options.VirtualHost,
                    DispatchConsumersAsync = true,
                };
            });
    }
}