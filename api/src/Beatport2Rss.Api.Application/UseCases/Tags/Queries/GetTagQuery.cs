using Beatport2Rss.Api.Application.Dtos.Tags;
using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Tags.Queries;

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