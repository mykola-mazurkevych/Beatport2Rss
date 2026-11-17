namespace Beatport2Rss.Common.Grpc.Options;

public sealed record GrpcOptions
{
    public required Uri Address { get; init; }
}