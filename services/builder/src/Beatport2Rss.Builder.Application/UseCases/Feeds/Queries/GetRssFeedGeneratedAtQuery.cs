using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Common;

namespace Beatport2Rss.Builder.Application.UseCases.Feeds.Queries;

public sealed record GetRssFeedGeneratedAtQuery(string Slug);

public sealed class GetRssFeedGeneratedAtQueryHandler(
    IRssFeedDocumentQueryRepository rssFeedDocumentQueryRepository)
{
    public Task<DateTimeOffset> Handle(
        GetRssFeedGeneratedAtQuery query,
        CancellationToken cancellationToken) =>
        rssFeedDocumentQueryRepository.GetGeneratedAtAsync(
            Slug.Create(query.Slug),
            cancellationToken);
}