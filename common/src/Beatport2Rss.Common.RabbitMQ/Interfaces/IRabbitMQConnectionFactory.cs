using RabbitMQ.Client;

namespace Beatport2Rss.Common.RabbitMQ.Interfaces;

internal interface IRabbitMQConnectionFactory
{
    IConnection CreateConnection();
}