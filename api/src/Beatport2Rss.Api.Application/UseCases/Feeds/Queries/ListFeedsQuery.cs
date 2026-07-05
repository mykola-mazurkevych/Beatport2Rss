using Beatport2Rss.Api.Application.Dtos.Feeds;
using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Querying.Paging;
using Beatport2Rss.Api.Application.Querying.Paging;
using Beatport2Rss.Api.Application.ReadModels.Feeds;
using Beatport2Rss.Api.Domain.Feeds;
using Beatport2Rss.Api.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Feeds.Queries;

public sealed record ListFeedsQuery(
    UserId UserId,
    Pagination Pagination) :
    IQuery<Result<Page<FeedPaginableDto>>>, IRequireValidation, IRequireUser;

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
    IQueryHandler<ListFeedsQuery, Result<Page<FeedPaginableDto>>>
{
    public async ValueTask<Result<Page<FeedPaginableDto>>> Handle(
        ListFeedsQuery query,
        CancellationToken cancellationToken)
    {
        var page = await pageBuilder.BuildAsync<FeedPaginableReadModel, FeedId>(
            feedQueryRepository.GetFeedPaginableReadModelsAsQueryable(query.UserId),
            query.Pagination,
            cancellationToken);

        var feedPaginableDtos = page.Dtos
            .Select(feedPaginableReadModel => new FeedPaginableDto(
                feedPaginableReadModel.Id,
                feedPaginableReadModel.Name,
                feedPaginableReadModel.Slug,
                feedPaginableReadModel.IsActive,
                feedPaginableReadModel.CreatedAt,
                feedPaginableReadModel.SubscriptionsCount))
            .ToList();

        return new Page<FeedPaginableDto>(feedPaginableDtos, page.Info);
    }
}