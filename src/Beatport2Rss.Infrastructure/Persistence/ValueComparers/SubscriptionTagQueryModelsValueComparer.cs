using Beatport2Rss.Infrastructure.QueryModels;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Beatport2Rss.Infrastructure.Persistence.ValueComparers;

internal sealed class SubscriptionTagQueryModelsValueComparer() :
    ValueComparer<IReadOnlyCollection<SubscriptionTagQueryModel>>(
        (left, right) =>
            left != null &&
            right != null &&
            left.SequenceEqual(right),
        value => value.Aggregate(0, (accumulator, item) => HashCode.Combine(accumulator, item.GetHashCode())),
        value => value.ToArray());