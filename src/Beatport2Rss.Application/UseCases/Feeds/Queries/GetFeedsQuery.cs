using Beatport2Rss.Application.Dtos.Feeds;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Pagination;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Pagination;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Feeds.Queries;

public sealed record GetFeedsQuery(
    UserId UserId,
    int? Size,
    string? Next,
    string? Previous) :
    IQuery<Result<Page<FeedPageDto>>>, IRequireValidation, IRequireUser;

internal sealed class GetFeedsQueryValidator :
    AbstractValidator<GetFeedsQuery>
{
    public GetFeedsQueryValidator(ICursorEncoder cursorEncoder)
    {
        RuleFor(q => q.Size).GreaterThan(0).When(q => q.Size.HasValue);
        RuleFor(q => q.Next).Must(next => cursorEncoder.TryDecode<FeedId>(next, out var _));
        RuleFor(q => q.Previous).Must(previous => cursorEncoder.TryDecode<FeedId>(previous, out var _));
    }
}

internal sealed class GetFeedsQueryHandler(
    IFeedQueryRepository feedQueryRepository,
    IPageBuilder pageBuilder) :
    IQueryHandler<GetFeedsQuery, Result<Page<FeedPageDto>>>
{
    public async ValueTask<Result<Page<FeedPageDto>>> Handle(
        GetFeedsQuery query,
        CancellationToken cancellationToken)
    {
        var page = await pageBuilder.BuildAsync<Feed, FeedId, FeedPageDto>(
            feedQueryRepository.Feeds,
            query.Size,
            query.Next,
            query.Previous,
            FeedPageDto.FromFeed,
            cancellationToken);

        return page;
    }
}