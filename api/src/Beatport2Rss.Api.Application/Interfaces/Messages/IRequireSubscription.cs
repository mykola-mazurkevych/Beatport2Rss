using Beatport2Rss.Api.Domain.Common.ValueObjects;

namespace Beatport2Rss.Api.Application.Interfaces.Messages;

public interface IRequireSubscription
{
    Slug SubscriptionSlug { get; }
}