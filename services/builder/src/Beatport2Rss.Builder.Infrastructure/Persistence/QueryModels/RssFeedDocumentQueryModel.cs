using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.QueryModels;

internal sealed record RssFeedDocumentQueryModel(
    FeedId Id,
    Slug Slug,
    DateTimeOffset GeneratedAt,
    string Content) :
    IQueryModel<FeedId>;