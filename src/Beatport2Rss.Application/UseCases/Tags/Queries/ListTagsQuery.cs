using Beatport2Rss.Application.Dtos.Tags;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Pagination;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Pagination;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Tags.Queries;

public sealed record ListTagsQuery(
    UserId UserId,
    PageNavigation PageNavigation) :
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

internal sealed class ListFeedsQueryHandler(
    ITagQueryRepository tagQueryRepository,
    IPageBuilder pageBuilder) :
    IQueryHandler<ListTagsQuery, Result<Page<TagPageDto>>>
{
    public async ValueTask<Result<Page<TagPageDto>>> Handle(
        ListTagsQuery query,
        CancellationToken cancellationToken)
    {
        var page = await pageBuilder.BuildAsync<Tag, TagId, TagPageDto>(
            tagQueryRepository.Tags.Where(t => t.UserId == query.UserId),
            query.PageNavigation,
            TagPageDto.FromTag,
            cancellationToken);

        return page;
    }
}