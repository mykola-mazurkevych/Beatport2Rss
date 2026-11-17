using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Infrastructure.Persistence.QueryModels;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Repositories;

internal sealed class RssFeedDocumentQueryRepository(
    IQueryable<RssFeedDocumentQueryModel> rssFeedDocumentQueryModels) :
    QueryRepository<RssFeedDocumentQueryModel, FeedId>(rssFeedDocumentQueryModels),
    IRssFeedDocumentQueryRepository
{
    public Task<DateTimeOffset> GetGeneratedAtAsync(Slug slug, CancellationToken cancellationToken = default) =>
        LoadAsync(
            queryModel => queryModel.Slug == slug,
            queryModel => queryModel.GeneratedAt,
            cancellationToken);

    public Task<string> GetContentAsync(Slug slug, CancellationToken cancellationToken = default) =>
        LoadAsync(
            queryModel => queryModel.Slug == slug,
            queryModel => queryModel.Content,
            cancellationToken);
}