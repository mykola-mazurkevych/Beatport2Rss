using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.ReadModels.Feeds;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Feeds.Queries;

public readonly record struct GetFeedQuery(
    Guid UserId,
    string? Slug) :
    IQuery<Result<FeedDetailsReadModel>>, IRequireActiveUser;

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
    IQueryHandler<GetFeedQuery, Result<FeedDetailsReadModel>>
{
    public async ValueTask<Result<FeedDetailsReadModel>> Handle(
        GetFeedQuery query,
        CancellationToken cancellationToken = default)
    {
        var userId = UserId.Create(query.UserId);
        var slug = Slug.Create(query.Slug);

        var feedDetailsQueryModel = await feedQueryRepository.LoadFeedDetailsQueryModelAsync(userId, slug, cancellationToken);

        return feedDetailsQueryModel;
    }
}