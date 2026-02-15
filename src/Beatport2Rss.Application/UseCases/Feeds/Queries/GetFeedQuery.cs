using Beatport2Rss.Application.Dtos.Feeds;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Feeds.Queries;

public sealed record GetFeedQuery(
    UserId UserId,
    Slug Slug) :
    IQuery<Result<FeedDto>>, IRequireUser, IRequireFeed;

internal sealed class GetFeedQueryHandler(
    IFeedQueryRepository feedQueryRepository) :
    IQueryHandler<GetFeedQuery, Result<FeedDto>>
{
    public async ValueTask<Result<FeedDto>> Handle(
        GetFeedQuery query,
        CancellationToken cancellationToken = default)
    {
        var readModel = await feedQueryRepository.LoadFeedDetailsReadModelAsync(query.UserId, query.Slug, cancellationToken);

        return new FeedDto(
            readModel.Id,
            readModel.Name,
            readModel.Slug,
            readModel.Owner,
            readModel.IsActive,
            readModel.CreatedAt);
    }
}