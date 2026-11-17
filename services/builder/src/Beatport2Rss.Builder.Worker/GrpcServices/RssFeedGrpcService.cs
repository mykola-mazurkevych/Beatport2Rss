using Beatport2Rss.Builder.Application.Dtos;
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
    public override async Task<GetRssFeedResponse> GetRssFeed(
        GetRssFeedRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.FeedId, out var feedId) || feedId == Guid.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "feed_id must be a non-empty GUID"));
        }

        var query = new GetRssFeedDocumentQuery(feedId, request.IfGeneratedAt?.ToDateTimeOffset());
        var document = await messageBus.InvokeAsync<RssFeedDocumentDto>(query, context.CancellationToken);

        return document.Status switch
        {
            RssFeedDocumentStatus.Found => new GetRssFeedResponse
            {
                GeneratedAt = Timestamp.FromDateTimeOffset(document.GeneratedAt!.Value),
                Content = document.Content,
            },
            RssFeedDocumentStatus.NotModified => new GetRssFeedResponse
            {
                NotModified = true,
                GeneratedAt = Timestamp.FromDateTimeOffset(document.GeneratedAt!.Value),
            },
            _ => throw new RpcException(new Status(StatusCode.NotFound, "RSS feed document was not found")),
        };
    }
}
