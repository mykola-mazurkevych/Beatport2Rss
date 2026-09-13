#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Common.Messaging.Interfaces;
using Beatport2Rss.Common.Messaging.Options;
using Beatport2Rss.Common.Messaging.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Common.Messaging;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMessaging(IConfiguration configuration) =>
            services
                .ConfigureOptions(configuration)
                .AddRabbitMq();

        private IServiceCollection AddRabbitMq() =>
            services
                .AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>()
                .AddSingleton<IPublisher, RabbitMqPublisher>()
                .AddTransient(typeof(IConsumer<>), typeof(RabbitMqConsumer<>));

        private IServiceCollection ConfigureOptions(IConfiguration configuration) =>
            services
                .Configure<RabbitMqOptions>(options => configuration.GetSection(nameof(RabbitMqOptions)).Bind(options))
                .Configure<QueueOptions>(options => configuration.GetSection(nameof(QueueOptions)).Bind(options));
    }
}