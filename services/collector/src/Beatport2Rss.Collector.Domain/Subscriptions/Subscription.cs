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

    public int SubscribersCount { get; private set; }

    public DateOnly? RefreshedAt { get; private set; }
    public DateOnly? RefreshAttemptedAt { get; private set; }
    public RefreshStatus? RefreshStatus { get; private set; }
    public string? FailReason { get; private set; }

    public static Subscription Create(
        SubscriptionId id,
        DateTimeOffset createdAt,
        SubscriptionType type,
        BeatportId beatportId,
        int subscribersCount,
        DateOnly? refreshedAt) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            Type = type,
            BeatportId = beatportId,
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

    public void RecordRefreshAttempt(DateOnly refreshAttemptedAt)
    {
        RefreshAttemptedAt = refreshAttemptedAt;
        RefreshStatus = null;
        FailReason = null;
    }

    public void RecordRefreshFailure(string failReason)
    {
        RefreshStatus = Subscriptions.RefreshStatus.Failed;
        FailReason = failReason;
    }

    public void MarkRefreshed()
    {
        RefreshStatus = Subscriptions.RefreshStatus.Refreshed;
        RefreshedAt = RefreshAttemptedAt;
    }
}