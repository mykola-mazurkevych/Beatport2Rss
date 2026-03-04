using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.Application.Interfaces.Services.Beatport;

public interface IBeatportUriBuilder
{
    Uri Build(
        BeatportSubscriptionType type,
        BeatportId id,
        BeatportSlug slug);
}