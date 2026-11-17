using Beatport2Rss.Api.Application.Interfaces.Services.Feeds;
using Beatport2Rss.Common.Domain.Common;

using RssFeedService;

namespace Beatport2Rss.Api.Infrastructure.Services.Feeds;

internal sealed class RssFeedDocumentGrpcClient(
    GrpcRssFeedService.GrpcRssFeedServiceClient client) :
    IRssFeedDocumentClient
{
    public async Task<DateTimeOffset> GetGeneratedAtAsync(Slug slug, CancellationToken cancellationToken = default)
    {
        var request = new GetRssFeedRequest { Slug = slug.Value };
        var response = await client.GetRssFeedGeneratedAtAsync(request, cancellationToken: cancellationToken);

        return response.GeneratedAt.ToDateTimeOffset();
    }

    public async Task<string> GetContentAsync(Slug slug, CancellationToken cancellationToken = default)
    {
        var request = new GetRssFeedRequest { Slug = slug.Value };
        var response = await client.GetRssFeedContentAsync(request, cancellationToken: cancellationToken);

        return response.Content;
    }
}