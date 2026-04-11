using Beatport2Rss.Application.Dtos.Tags;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Tags.Queries;

public sealed record GetTagQuery(
    UserId UserId,
    Slug TagSlug) :
    IQuery<Result<TagDto>>, IRequireUser, IRequireTag;

internal sealed class GetTagQueryHandler(
    ITagQueryRepository tagQueryRepository) :
    IQueryHandler<GetTagQuery, Result<TagDto>>
{
    public async ValueTask<Result<TagDto>> Handle(
        GetTagQuery query,
        CancellationToken cancellationToken)
    {
        var tagDetails = await tagQueryRepository.LoadTagDetailsAsync(query.UserId, query.TagSlug, cancellationToken);

        return new TagDto(
            tagDetails.Id,
            tagDetails.Name,
            tagDetails.Slug,
            tagDetails.CreatedAt);
    }
}