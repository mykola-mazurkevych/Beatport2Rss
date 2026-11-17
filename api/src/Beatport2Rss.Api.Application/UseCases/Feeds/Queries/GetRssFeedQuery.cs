using Beatport2Rss.Api.Application.Dtos.Feeds;
using Beatport2Rss.Api.Application.Extensions;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Services.Feeds;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.FluentResults.Extensions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Feeds.Queries;

public sealed record GetRssFeedQuery(
    Slug Slug,
    DateTimeOffset? IfModifiedSince) :
    IQuery<Result<RssFeedDocumentDto>>;

internal sealed class GetRssFeedQueryHandler(
    IFeedQueryRepository feedQueryRepository,
    IRssFeedDocumentClient rssFeedDocumentClient) :
    IQueryHandler<GetRssFeedQuery, Result<RssFeedDocumentDto>>
{
    public async ValueTask<Result<RssFeedDocumentDto>> Handle(
        GetRssFeedQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!await feedQueryRepository.ExistsAsync(query.Slug, cancellationToken))
        {
            return Result.NotFound($"Feed '{query.Slug}' was not found.");
        }

        var generatedAt = await rssFeedDocumentClient.GetGeneratedAtAsync(query.Slug, cancellationToken);
        var lastModified = generatedAt.ToHttpDate();

        if (query.IfModifiedSince is { } ifModifiedSince && ifModifiedSince.ToHttpDate() >= lastModified)
        {
            return Result.NotModified();
        }

        var content = await rssFeedDocumentClient.GetContentAsync(query.Slug, cancellationToken);

        var rssFeedDocumentDto = new RssFeedDocumentDto
        {
            LastModified = lastModified,
            Content = content,
        };

        return Result.Ok(rssFeedDocumentDto);
    }
}