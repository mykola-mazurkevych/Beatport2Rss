using Beatport2Rss.Api.Application.Dtos.Subscriptions;
using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Beatport.Interfaces;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Subscriptions.Queries;

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
            subscriptionDetails.Type,
            subscriptionDetails.Name,
            subscriptionDetails.Slug,
            beatportUriBuilder.Build(
                subscriptionDetails.BeatportId.Value,
                subscriptionDetails.BeatportSlug.Value),
            subscriptionDetails.ImageUri,
            subscriptionDetails.Country,
            subscriptionDetails.Tags
                .Select(subscriptionTag => new SubscriptionTagDto(
                    subscriptionTag.Name,
                    subscriptionTag.Slug)));
    }
}