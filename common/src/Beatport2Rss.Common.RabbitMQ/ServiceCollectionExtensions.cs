#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Common.RabbitMQ.Interfaces;
using Beatport2Rss.Common.RabbitMQ.Options;
using Beatport2Rss.Common.RabbitMQ.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Common.RabbitMQ;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRabbitMQ(IConfiguration configuration) =>
            services
                .ConfigureOptions(configuration)
                .AddSingleton<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>()
                .AddSingleton<IPublisher, RabbitMQPublisher>()
                .AddTransient(typeof(IConsumer<>), typeof(RabbitMQConsumer<>));

        private IServiceCollection ConfigureOptions(IConfiguration configuration) =>
            services
                .Configure<RabbitMQOptions>(options => configuration.GetSection(nameof(RabbitMQOptions)).Bind(options))
                .Configure<QueueOptions>(options => configuration.GetSection(nameof(QueueOptions)).Bind(options));
    }
}