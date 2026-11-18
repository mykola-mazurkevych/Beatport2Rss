using Beatport2Rss.Builder.Application.UseCases.Feeds.Queries;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using RssFeedService;

using Wolverine;

namespace Beatport2Rss.Builder.Worker.GrpcServices;

internal sealed class RssFeedGrpcService(
    IMessageBus messageBus) :
    GrpcRssFeedService.GrpcRssFeedServiceBase
{
    public override async Task<GetRssFeedGeneratedAtResponse> GetRssFeedGeneratedAt(
        GetRssFeedRequest request,
        ServerCallContext context)
    {
        var query = new GetRssFeedGeneratedAtQuery(request.Slug);
        var generatedAt = await messageBus.InvokeAsync<DateTimeOffset>(query, context.CancellationToken);

        return new GetRssFeedGeneratedAtResponse
        {
            GeneratedAt = Timestamp.FromDateTimeOffset(generatedAt),
        };
    }

    public override async Task<GetRssFeedContentResponse> GetRssFeedContent(
        GetRssFeedRequest request,
        ServerCallContext context)
    {
        var query = new GetRssFeedDocumentContentQuery(request.Slug);
        var content = await messageBus.InvokeAsync<string>(query, context.CancellationToken);

        return new GetRssFeedContentResponse
        {
            Content = content,
        };
    }
}