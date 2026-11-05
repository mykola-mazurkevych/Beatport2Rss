using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Interfaces;
using Beatport2Rss.Common.Domain.Subscriptions;

namespace Beatport2Rss.Collector.Domain.Subscriptions;

public sealed class Subscription :
    IAggregateRoot<SubscriptionId>
{
    private Subscription()
    {
    }

    public SubscriptionId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public SubscriptionType Type { get; private set; }

    public BeatportId BeatportId { get; private set; }
    public BeatportSlug BeatportSlug { get; private set; }

    public int SubscribersCount { get; private set; }

    public DateOnly? RefreshedAt { get; private set; }
    public DateTimeOffset? RefreshAttemptedAt { get; private set; }
    public RefreshStatus? RefreshStatus { get; private set; }
    public string? FailReason { get; private set; }

    public static Subscription Create(
        SubscriptionId id,
        DateTimeOffset createdAt,
        SubscriptionType type,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        int subscribersCount,
        DateOnly? refreshedAt) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            Type = type,
            BeatportId = beatportId,
            BeatportSlug = beatportSlug,
            SubscribersCount = subscribersCount,
            RefreshedAt = refreshedAt,
        };

    public void IncrementSubscribersCount() => SubscribersCount++;

    public bool DecrementSubscribersCount()
    {
        if (SubscribersCount == 0)
        {
            return false;
        }

        SubscribersCount--;
        return true;
    }

    public void RecordRefreshAttempt(DateTimeOffset attemptedAt)
    {
        RefreshAttemptedAt = attemptedAt;
        RefreshStatus = null;
        FailReason = null;
    }

    public void RecordRefreshFailure(string reason)
    {
        RefreshStatus = Subscriptions.RefreshStatus.Failed;
        FailReason = reason;
    }

    public void MarkRefreshed()
    {
        if (RefreshAttemptedAt is null)
        {
            throw new InvalidOperationException("Refresh attempt must be recorded before marking the subscription as refreshed");
        }

        RefreshStatus = Subscriptions.RefreshStatus.Refreshed;
        RefreshedAt = DateOnly.FromDateTime(RefreshAttemptedAt.Value.UtcDateTime);
    }
}