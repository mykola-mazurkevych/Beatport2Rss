using Beatport2Rss.Builder.Application.Dtos;
using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Feeds;

namespace Beatport2Rss.Builder.Application.UseCases.Feeds.Queries;

public sealed record GetRssFeedDocumentQuery(
    Guid FeedId,
    DateTimeOffset? IfGeneratedAt);

public sealed class GetRssFeedDocumentQueryHandler(
    IRssFeedDocumentQueryRepository rssFeedDocumentQueryRepository)
{
    public async Task<RssFeedDocumentDto> Handle(
        GetRssFeedDocumentQuery query,
        CancellationToken cancellationToken)
    {
        var feedId = FeedId.Create(query.FeedId);

        if (query.IfGeneratedAt is { } ifGeneratedAt)
        {
            var generatedAt = await rssFeedDocumentQueryRepository.FindGeneratedAtAsync(feedId, cancellationToken);
            if (generatedAt is null)
            {
                return RssFeedDocumentDto.NotFound();
            }

            if (generatedAt.Value == ifGeneratedAt)
            {
                return RssFeedDocumentDto.NotModified(generatedAt.Value);
            }
        }

        var document = await rssFeedDocumentQueryRepository.FindContentAsync(feedId, cancellationToken);

        return document is { } found
            ? RssFeedDocumentDto.Found(found.GeneratedAt, found.Content)
            : RssFeedDocumentDto.NotFound();
    }
}
