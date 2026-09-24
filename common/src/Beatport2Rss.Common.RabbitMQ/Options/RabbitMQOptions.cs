namespace Beatport2Rss.Common.RabbitMQ.Options;

public sealed record RabbitMQOptions
{
    public required string HostName { get; init; }
    public required int Port { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
    public required string VirtualHost { get; init; }
}