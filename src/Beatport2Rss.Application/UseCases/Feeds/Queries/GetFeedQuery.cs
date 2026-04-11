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
    Slug FeedSlug) :
    IQuery<Result<FeedDto>>, IRequireUser, IRequireFeed;

internal sealed class GetFeedQueryHandler(
    IFeedQueryRepository feedQueryRepository) :
    IQueryHandler<GetFeedQuery, Result<FeedDto>>
{
    public async ValueTask<Result<FeedDto>> Handle(
        GetFeedQuery query,
        CancellationToken cancellationToken = default)
    {
        var feedDetails = await feedQueryRepository.LoadFeedDetailsAsync(query.UserId, query.FeedSlug, cancellationToken);

        return new FeedDto(
            feedDetails.Id,
            feedDetails.Name,
            feedDetails.Slug,
            feedDetails.IsActive,
            feedDetails.CreatedAt,
            feedDetails.SubscriptionsCount);
    }
}