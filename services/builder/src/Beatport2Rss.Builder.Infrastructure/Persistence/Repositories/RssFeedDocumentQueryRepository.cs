using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Domain.Feeds;
using Beatport2Rss.Common.Domain.Feeds;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Repositories;

internal sealed class RssFeedDocumentQueryRepository(DbSet<RssFeedDocument> rssFeedDocuments) :
    IRssFeedDocumentQueryRepository
{
    public async Task<DateTimeOffset?> FindGeneratedAtAsync(
        FeedId feedId,
        CancellationToken cancellationToken = default) =>
        await rssFeedDocuments
            .AsNoTracking()
            .Where(document => document.Id == feedId)
            .Select(document => (DateTimeOffset?)document.GeneratedAt)
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<(DateTimeOffset GeneratedAt, string Content)?> FindContentAsync(
        FeedId feedId,
        CancellationToken cancellationToken = default)
    {
        var document = await rssFeedDocuments
            .AsNoTracking()
            .Where(document => document.Id == feedId)
            .Select(document => new { document.GeneratedAt, document.Content })
            .SingleOrDefaultAsync(cancellationToken);

        return document is null ? null : (document.GeneratedAt, document.Content);
    }
}
