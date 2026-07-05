using Beatport2Rss.ReleaseCollector.Domain.Common.Subscriptions;
using Beatport2Rss.ReleaseCollector.Domain.Common.ValueObjects;

namespace Beatport2Rss.ReleaseCollector.Domain.Labels;

public sealed class Label :
    Subscription<LabelId, LabelName>
{
    private Label()
    {
    }

    private Label(
        LabelId id,
        DateTimeOffset createdAt,
        LabelName name,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        int subscribersCount,
        DateTimeOffset? refreshedAt) :
        base(id,
            createdAt,
            name,
            beatportId,
            beatportSlug,
            subscribersCount,
            refreshedAt)
    {
    }

    public static Label Create(
        LabelId id,
        DateTimeOffset createdAt,
        LabelName name,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        int subscribersCount,
        DateTimeOffset? refreshedAt) =>
        new(id,
            createdAt,
            name,
            beatportId,
            beatportSlug,
            subscribersCount,
            refreshedAt);
}