using Beatport2Rss.Common.Messaging.Interfaces;
using Beatport2Rss.Common.Messaging.Options;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace Beatport2Rss.Common.Messaging.Services;

internal sealed class RabbitMqConnectionFactory :
    IRabbitMqConnectionFactory
{
    private readonly ConnectionFactory _connectionFactory;

    public RabbitMqConnectionFactory(IOptions<RabbitMqOptions> options)
    {
        var rabbitMqOptions = options.Value;
        _connectionFactory = new ConnectionFactory
        {
            HostName = rabbitMqOptions.HostName,
            Port = rabbitMqOptions.Port,
            UserName = rabbitMqOptions.UserName,
            Password = rabbitMqOptions.Password,
            VirtualHost = rabbitMqOptions.VirtualHost,
            DispatchConsumersAsync = true,
        };
    }

    public IConnection CreateConnection() =>
        _connectionFactory.CreateConnection();
}