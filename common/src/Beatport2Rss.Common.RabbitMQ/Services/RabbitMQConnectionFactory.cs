using Beatport2Rss.Common.RabbitMQ.Interfaces;
using Beatport2Rss.Common.RabbitMQ.Options;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace Beatport2Rss.Common.RabbitMQ.Services;

internal sealed class RabbitMQConnectionFactory(IOptions<RabbitMQOptions> options) :
    IRabbitMQConnectionFactory
{
    private readonly ConnectionFactory _connectionFactory = new()
    {
        HostName = options.Value.HostName,
        Port = options.Value.Port,
        UserName = options.Value.UserName,
        Password = options.Value.Password,
        VirtualHost = options.Value.VirtualHost,
        DispatchConsumersAsync = true,
    };

    public IConnection CreateConnection() =>
        _connectionFactory.CreateConnection();
}