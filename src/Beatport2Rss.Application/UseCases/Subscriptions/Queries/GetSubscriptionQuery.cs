using Beatport2Rss.Application.Dtos.Subscriptions;
using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Beatport;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.SharedKernel.Extensions;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Subscriptions.Queries;

public sealed record GetSubscriptionQuery(
    BeatportSubscriptionType Type,
    int BeatportId,
    string BeatportSlug) :
    IQuery<Result<SubscriptionDto>>, IRequireValidation;

internal sealed class GetSubscriptionQueryValidator :
    AbstractValidator<GetSubscriptionQuery>
{
    public GetSubscriptionQueryValidator()
    {
        RuleFor(q => q.Type).IsInEnum();
        RuleFor(q => q.BeatportId).GreaterThan(0);
        RuleFor(q => q.BeatportSlug).IsBeatportSlug();
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
        var beatportId = BeatportId.Create(query.BeatportId);
        var beatportSlug = BeatportSlug.Create(query.BeatportSlug);
        var readModel = await subscriptionQueryRepository.LoadAsync(query.Type, beatportId, beatportSlug, cancellationToken);

        if (readModel is null)
        {
            return Result.NotFound("Subscription not found");
        }

        return new SubscriptionDto(
            readModel.Id,
            readModel.Name,
            readModel.BeatportId,
            readModel.BeatportSlug,
            beatportUriBuilder.Build(readModel.BeatportType, readModel.BeatportId, readModel.BeatportSlug),
            readModel.ImageUri,
            readModel.CreatedAt,
            readModel.RefreshedAt);
    }
}