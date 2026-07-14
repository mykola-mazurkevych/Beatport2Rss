using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.Application.Interfaces.Messages;

public interface IRequireSubscription
{
    Slug SubscriptionSlug { get; }
}