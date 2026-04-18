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
    IQuery<Result<Page<TagPageDto>>>, IRequireValidation, IRequireUser;

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
    IQueryHandler<ListTagsQuery, Result<Page<TagPageDto>>>
{
    public async ValueTask<Result<Page<TagPageDto>>> Handle(
        ListTagsQuery query,
        CancellationToken cancellationToken)
    {
        var page = await pageBuilder.BuildAsync<TagPageReadModel, TagId>(
            tagQueryRepository.GetTagPageReadModelsAsQueryable(query.UserId),
            query.Pagination,
            cancellationToken);

        var tagPageDtos = page.Dtos
            .Select(readModel => new TagPageDto(
                readModel.Id,
                readModel.Name,
                readModel.Slug,
                readModel.CreatedAt))
            .ToList();

        return new Page<TagPageDto>(tagPageDtos, page.Info);
    }
}