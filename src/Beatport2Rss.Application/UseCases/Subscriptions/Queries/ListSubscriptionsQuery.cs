using Beatport2Rss.Application.Dtos.Subscriptions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Querying.Paging;
using Beatport2Rss.Application.Interfaces.Services.Beatport;
using Beatport2Rss.Application.Querying.Paging;
using Beatport2Rss.Application.ReadModels.Subscriptions;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Subscriptions.Queries;

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
                subscriptionPaginableReadModel.Name,
                subscriptionPaginableReadModel.Slug,
                subscriptionPaginableReadModel.BeatportType,
                beatportUriBuilder.Build(
                    subscriptionPaginableReadModel.BeatportType,
                    subscriptionPaginableReadModel.BeatportId,
                    subscriptionPaginableReadModel.BeatportSlug),
                subscriptionPaginableReadModel.ImageUri,
                subscriptionPaginableReadModel.Tags
                    .Select(subscriptionTagReadModel => new SubscriptionTagDto(
                        subscriptionTagReadModel.Name,
                        subscriptionTagReadModel.Slug)),
                subscriptionPaginableReadModel.RefreshedAt))
            .ToList();

        return new Page<SubscriptionPaginableDto>(subscriptionPaginableDtos, page.Info);
    }
}