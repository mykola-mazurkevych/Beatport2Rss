using Beatport2Rss.Domain.Common.ValueObjects;

namespace Beatport2Rss.Application.Interfaces.Messages;

public interface IRequireSubscription
{
    Slug SubscriptionSlug { get; }
}