using Beatport2Rss.Builder.Domain.Common.Subscriptions;
using Beatport2Rss.Builder.Domain.Common.ValueObjects;

namespace Beatport2Rss.Builder.Domain.Labels;

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
        Uri beatportUri):
        base(id,
            createdAt,
            name,
            beatportId,
            beatportUri)
    {
    }

    public static Label Create(
        LabelId id,
        DateTimeOffset createdAt,
        LabelName name,
        BeatportId beatportId,
        Uri beatportUri) =>
        new(id,
            createdAt,
            name,
            beatportId,
            beatportUri);
}