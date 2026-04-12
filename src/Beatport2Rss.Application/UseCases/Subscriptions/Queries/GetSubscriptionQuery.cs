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
        var subscriptionDetails = await subscriptionQueryRepository.LoadWithUserTagsAsync(query.SubscriptionSlug, query.UserId, cancellationToken);

        return new SubscriptionDto(
            subscriptionDetails.Id,
            subscriptionDetails.Name,
            subscriptionDetails.Slug,
            beatportUriBuilder.Build(subscriptionDetails.BeatportType, subscriptionDetails.BeatportId, subscriptionDetails.BeatportSlug),
            subscriptionDetails.ImageUri,
            subscriptionDetails.Tags.Select(subscriptionTagDetails => new SubscriptionTagDto(subscriptionTagDetails.Name, subscriptionTagDetails.Slug)),
            subscriptionDetails.CreatedAt,
            subscriptionDetails.RefreshedAt);
    }
}