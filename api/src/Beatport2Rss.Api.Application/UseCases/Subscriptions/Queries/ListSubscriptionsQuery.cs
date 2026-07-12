using Beatport2Rss.Api.Application.Dtos.Subscriptions;
using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Querying.Paging;
using Beatport2Rss.Api.Application.Querying.Paging;
using Beatport2Rss.Api.Application.ReadModels.Subscriptions;
using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Beatport.Interfaces;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Subscriptions.Queries;

public sealed record ListSubscriptionsQuery(
    UserId UserId,
    Pagination Pagination) :
    IQuery<Result<Page<SubscriptionPaginableDto>>>, IRequireValidation, IRequireUser;

internal sealed class ListSubscriptionsQueryValidator :
    AbstractValidator<ListSubscriptionsQuery>;

internal sealed class ListSubscriptionsQueryHandler(
    IBeatportUriBuilder beatportUriBuilder,
    ISubscriptionQueryRepository subscriptionQueryRepository,
    IPageBuilder pageBuilder) :
    IQueryHandler<ListSubscriptionsQuery, Result<Page<SubscriptionPaginableDto>>>
{
    public async ValueTask<Result<Page<SubscriptionPaginableDto>>> Handle(
        ListSubscriptionsQuery query,
        CancellationToken cancellationToken)
    {
        var page = await pageBuilder.BuildAsync<SubscriptionPaginableReadModel, SubscriptionId>(
            subscriptionQueryRepository.GetSubscriptionPaginableReadModelsAsQueryable(query.UserId),
            query.Pagination,
            cancellationToken);

        var subscriptionPaginableDtos = page.Dtos
            .Select(subscriptionPaginableReadModel => new SubscriptionPaginableDto(
                subscriptionPaginableReadModel.Id,
                subscriptionPaginableReadModel.Type,
                subscriptionPaginableReadModel.Name,
                subscriptionPaginableReadModel.Slug,
                beatportUriBuilder.Build(
                    subscriptionPaginableReadModel.BeatportId.Value,
                    subscriptionPaginableReadModel.BeatportSlug.Value),
                subscriptionPaginableReadModel.ImageUri,
                subscriptionPaginableReadModel.Country,
                subscriptionPaginableReadModel.SubscribersCount,
                subscriptionPaginableReadModel.Tags
                    .Select(subscriptionTagReadModel => new SubscriptionTagDto(
                        subscriptionTagReadModel.Name,
                        subscriptionTagReadModel.Slug))))
            .ToList();

        return new Page<SubscriptionPaginableDto>(subscriptionPaginableDtos, page.Info);
    }
}