using Beatport2Rss.Application.Dtos.Feeds;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Querying.Paging;
using Beatport2Rss.Application.Querying.Paging;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Feeds.Queries;

public sealed record ListFeedsQuery(
    UserId UserId,
    Pagination Pagination) :
    IQuery<Result<Page<FeedPageDto>>>, IRequireValidation, IRequireUser;

internal sealed class ListFeedsQueryValidator :
    AbstractValidator<ListFeedsQuery>
{
    // public ListFeedsQueryValidator(ICursorEncoder cursorEncoder)
    // {
    //     RuleFor(q => q.Size).GreaterThan(0).When(q => q.Size.HasValue);
    //     RuleFor(q => q.Next).Must(next => cursorEncoder.TryDecode<FeedId>(next, out var _));
    //     RuleFor(q => q.Previous).Must(previous => cursorEncoder.TryDecode<FeedId>(previous, out var _));
    // }
}

internal sealed class ListFeedsQueryHandler(
    IFeedQueryRepository feedQueryRepository,
    IPageBuilder pageBuilder) :
    IQueryHandler<ListFeedsQuery, Result<Page<FeedPageDto>>>
{
    public async ValueTask<Result<Page<FeedPageDto>>> Handle(
        ListFeedsQuery query,
        CancellationToken cancellationToken)
    {
        var page = await pageBuilder.BuildAsync<Feed, FeedId, FeedPageDto>(
            feedQueryRepository.Feeds.Where(f => f.UserId == query.UserId),
            query.Pagination,
            FeedPageDto.Create,
            cancellationToken);

        return page;
    }
}