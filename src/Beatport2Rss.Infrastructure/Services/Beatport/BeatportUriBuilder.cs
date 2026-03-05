using Beatport2Rss.Application.Interfaces.Services.Beatport;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.Infrastructure.Services.Beatport;

internal sealed class BeatportUriBuilder :
    IBeatportUriBuilder
{
    private const string BaseUriString = "https://www.beatport.com";

    public Uri Build(BeatportSubscriptionType type, BeatportId id, BeatportSlug slug) =>
        new($"{BaseUriString}/{slug}/{id}");
}