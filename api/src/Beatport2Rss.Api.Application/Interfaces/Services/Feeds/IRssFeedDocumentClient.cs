using Beatport2Rss.Common.Domain.Common;

namespace Beatport2Rss.Api.Application.Interfaces.Services.Feeds;

public interface IRssFeedDocumentClient
{
    Task<DateTimeOffset> GetGeneratedAtAsync(Slug slug, CancellationToken cancellationToken = default);
    Task<string> GetContentAsync(Slug slug, CancellationToken cancellationToken = default);
}