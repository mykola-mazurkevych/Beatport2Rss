using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Feeds;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using RssFeedService;

namespace Beatport2Rss.Builder.Worker.GrpcServices;

internal sealed class RssFeedGrpcService(
    IRssFeedDocumentQueryRepository rssFeedDocumentQueryRepository) :
    GrpcRssFeedService.GrpcRssFeedServiceBase
{
    public override async Task<GetRssFeedResponse> GetRssFeed(
        GetRssFeedRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.FeedId, out var feedGuid) || feedGuid == Guid.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "feed_id must be a non-empty GUID"));
        }

        var feedId = FeedId.Create(feedGuid);

        if (request.IfGeneratedAt is not null)
        {
            var generatedAt = await rssFeedDocumentQueryRepository.FindGeneratedAtAsync(feedId, context.CancellationToken);
            if (generatedAt is null)
            {
                throw NotFound();
            }

            if (generatedAt.Value == request.IfGeneratedAt.ToDateTimeOffset())
            {
                return new GetRssFeedResponse
                {
                    NotModified = true,
                    GeneratedAt = Timestamp.FromDateTimeOffset(generatedAt.Value),
                };
            }
        }

        var document = await rssFeedDocumentQueryRepository.FindContentAsync(feedId, context.CancellationToken)
            ?? throw NotFound();

        return new GetRssFeedResponse
        {
            GeneratedAt = Timestamp.FromDateTimeOffset(document.GeneratedAt),
            Content = document.Content,
        };
    }

    private static RpcException NotFound() =>
        new(new Status(StatusCode.NotFound, "RSS feed document was not found"));
}
