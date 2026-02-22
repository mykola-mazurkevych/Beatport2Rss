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
    Slug Slug) :
    IQuery<Result<TagDto>>, IRequireUser, IRequireTag;

internal sealed class GetTagQueryHandler(
    ITagQueryRepository tagQueryRepository) :
    IQueryHandler<GetTagQuery, Result<TagDto>>
{
    public async ValueTask<Result<TagDto>> Handle(
        GetTagQuery query,
        CancellationToken cancellationToken)
    {
        var readModel = await tagQueryRepository.LoadTagDetailsReadModelAsync(query.UserId, query.Slug, cancellationToken);

        return new TagDto(
            readModel.Id,
            readModel.Name,
            readModel.Slug,
            readModel.CreatedAt);
    }
}