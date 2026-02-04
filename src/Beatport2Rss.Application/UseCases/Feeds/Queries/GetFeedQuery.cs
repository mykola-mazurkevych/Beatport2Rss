using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Feeds.Queries;

public sealed record GetFeedResponse(
    FeedId Id,
    Slug Slug,
    string Name,
    string? Owner,
    bool IsActive,
    DateTimeOffset CreatedAt);

public sealed record GetFeedQuery(
    Guid UserId,
    string? Slug) :
    IQuery<Result<GetFeedResponse>>, IRequireActiveUser;

internal sealed class GetFeedQueryValidator :
    AbstractValidator<GetFeedQuery>
{
    public GetFeedQueryValidator()
    {
        RuleFor(q => q.UserId).IsUserId();
        RuleFor(q => q.Slug).IsSlug();
    }
}

internal sealed class GetFeedQueryHandler(
    IFeedQueryRepository feedQueryRepository) :
    IQueryHandler<GetFeedQuery, Result<GetFeedResponse>>
{
    public async ValueTask<Result<GetFeedResponse>> Handle(
        GetFeedQuery query,
        CancellationToken cancellationToken = default)
    {
        var userId = UserId.Create(query.UserId);
        var slug = Slug.Create(query.Slug);

        var readModel = await feedQueryRepository.LoadFeedDetailsReadModelAsync(userId, slug, cancellationToken);

        return new GetFeedResponse(
            readModel.Id,
            readModel.Slug,
            readModel.Name,
            readModel.Owner,
            readModel.IsActive,
            readModel.CreatedAt);
    }
}