using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Common;

namespace Beatport2Rss.Builder.Application.UseCases.Feeds.Queries;

public sealed record GetRssFeedDocumentContentQuery(string Slug);

public sealed class GetRssFeedDocumentContentQueryHandler(
    IRssFeedDocumentQueryRepository rssFeedDocumentQueryRepository)
{
    public Task<string> HandleAsync(
        GetRssFeedDocumentContentQuery query,
        CancellationToken cancellationToken) =>
        rssFeedDocumentQueryRepository.GetContentAsync(
            Slug.Create(query.Slug),
            cancellationToken);
}