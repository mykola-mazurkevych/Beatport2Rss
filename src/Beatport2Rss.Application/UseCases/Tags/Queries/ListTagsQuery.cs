using Beatport2Rss.Application.Dtos.Tags;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Querying.Paging;
using Beatport2Rss.Application.Querying.Paging;
using Beatport2Rss.Application.ReadModels.Tags;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Tags.Queries;

public sealed record ListTagsQuery(
    UserId UserId,
    Pagination Pagination) :
    IQuery<Result<Page<TagPaginableDto>>>, IRequireValidation, IRequireUser;

internal sealed class ListTagsQueryValidator :
    AbstractValidator<ListTagsQuery>
{
    // public ListTagsQueryValidator(ICursorEncoder cursorEncoder)
    // {
    //     RuleFor(q => q.Size).GreaterThan(0).When(q => q.Size.HasValue);
    //     RuleFor(q => q.Next).Must(next => cursorEncoder.TryDecode<TagId>(next, out var _));
    //     RuleFor(q => q.Previous).Must(previous => cursorEncoder.TryDecode<TagId>(previous, out var _));
    // }
}

internal sealed class ListTagsQueryHandler(
    ITagQueryRepository tagQueryRepository,
    IPageBuilder pageBuilder) :
    IQueryHandler<ListTagsQuery, Result<Page<TagPaginableDto>>>
{
    public async ValueTask<Result<Page<TagPaginableDto>>> Handle(
        ListTagsQuery query,
        CancellationToken cancellationToken)
    {
        var page = await pageBuilder.BuildAsync<TagPaginableReadModel, TagId>(
            tagQueryRepository.GetTagPaginableReadModelsAsQueryable(query.UserId),
            query.Pagination,
            cancellationToken);

        var tagPaginableDtos = page.Dtos
            .Select(tagPaginableReadModel => new TagPaginableDto(
                tagPaginableReadModel.Id,
                tagPaginableReadModel.Name,
                tagPaginableReadModel.Slug,
                tagPaginableReadModel.CreatedAt))
            .ToList();

        return new Page<TagPaginableDto>(tagPaginableDtos, page.Info);
    }
}