using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;

namespace Beatport2Rss.Builder.Application.Dtos;

public sealed record LabelDto(
    BeatportId BeatportId,
    SubscriptionName Name);