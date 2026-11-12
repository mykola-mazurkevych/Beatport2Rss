using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;

using FluentResults;

namespace Beatport2Rss.Collector.Application.Interfaces.Services;

internal interface ISubscriptionService
{
    Task<Result> EnsureSubscriptionExistsAsync(
        SubscriptionType subscriptionType,
        BeatportId beatportId,
        CancellationToken cancellationToken = default);
}