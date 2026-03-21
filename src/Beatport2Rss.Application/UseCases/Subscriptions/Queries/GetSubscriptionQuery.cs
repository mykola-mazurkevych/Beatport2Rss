using Beatport2Rss.Application.Dtos.Subscriptions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Beatport;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Subscriptions.Queries;

public sealed record GetSubscriptionQuery(
    UserId UserId,
    Slug SubscriptionSlug) :
    IQuery<Result<SubscriptionDto>>, IRequireValidation, IRequireSubscription;

internal sealed class GetSubscriptionQueryValidator :
    AbstractValidator<GetSubscriptionQuery>
{
    public GetSubscriptionQueryValidator()
    {
        RuleFor(q => q.SubscriptionSlug).NotEmpty();
    }
}

internal sealed class GetSubscriptionQueryHandler(
    IBeatportUriBuilder beatportUriBuilder,
    ISubscriptionQueryRepository subscriptionQueryRepository) :
    IQueryHandler<GetSubscriptionQuery, Result<SubscriptionDto>>
{
    public async ValueTask<Result<SubscriptionDto>> Handle(
        GetSubscriptionQuery query,
        CancellationToken cancellationToken)
    {
        var readModel = await subscriptionQueryRepository.LoadWithUserTagsAsync(query.SubscriptionSlug, query.UserId, cancellationToken);

        return new SubscriptionDto(
            readModel.Id,
            readModel.Name,
            readModel.Slug,
            beatportUriBuilder.Build(readModel.BeatportType, readModel.BeatportId, readModel.BeatportSlug),
            readModel.ImageUri,
            readModel.Tags.Select(t => new SubscriptionTagDto(t.Name, t.Slug)),
            readModel.CreatedAt,
            readModel.RefreshedAt);
    }
}