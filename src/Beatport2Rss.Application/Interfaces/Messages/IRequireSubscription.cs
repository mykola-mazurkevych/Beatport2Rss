using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.Application.Interfaces.Messages;

public interface IRequireSubscription
{
    BeatportSubscriptionType BeatportType { get; }
    BeatportId BeatportId { get; }
    BeatportSlug BeatportSlug { get; }
}