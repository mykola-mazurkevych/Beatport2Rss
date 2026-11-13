using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Interfaces;
using Beatport2Rss.Common.Domain.Releases;

namespace Beatport2Rss.Collector.Domain.Releases;

public sealed class Release :
    IAggregateRoot<ReleaseId>
{
    private Release()
    {
    }

    public ReleaseId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public BeatportId BeatportId { get; private set; }

    public static Release Create(
        ReleaseId id,
        DateTimeOffset createdAt,
        BeatportId beatportId) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            BeatportId = beatportId,
        };
}