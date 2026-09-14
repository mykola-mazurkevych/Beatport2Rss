using RabbitMQ.Client;

namespace Beatport2Rss.Common.Messaging.Interfaces;

internal interface IRabbitMqConnectionFactory
{
    IConnection CreateConnection();
}