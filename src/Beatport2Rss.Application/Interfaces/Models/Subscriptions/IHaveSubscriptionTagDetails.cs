using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;

namespace Beatport2Rss.Application.Interfaces.Models.Subscriptions;

public interface IHaveSubscriptionTagDetails
{
    TagName Name { get; }
    Slug Slug { get; }
}