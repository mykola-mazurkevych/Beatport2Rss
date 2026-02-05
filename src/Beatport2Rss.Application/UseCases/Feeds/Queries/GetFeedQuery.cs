using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Feeds.Queries;

public sealed record GetFeedResponse(
    FeedId FeedId,
    Slug Slug,
    string Name,
    string? Owner,
    bool IsActive,
    DateTimeOffset CreatedAt);

public sealed record GetFeedQuery(
    UserId UserId,
    Slug Slug) :
    IQuery<Result<GetFeedResponse>>, IRequireSlug;

internal sealed class GetFeedQueryHandler(
    IFeedQueryRepository feedQueryRepository) :
    IQueryHandler<GetFeedQuery, Result<GetFeedResponse>>
{
    public async ValueTask<Result<GetFeedResponse>> Handle(
        GetFeedQuery query,
        CancellationToken cancellationToken = default)
    {
        var readModel = await feedQueryRepository.LoadFeedDetailsReadModelAsync(query.UserId, query.Slug, cancellationToken);

        return new GetFeedResponse(
            readModel.FeedId,
            readModel.Slug,
            readModel.Name,
            readModel.Owner,
            readModel.IsActive,
            readModel.CreatedAt);
    }
}