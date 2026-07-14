using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.Application.Interfaces.Messages;

public interface IRequireFeed
{
    Slug FeedSlug { get; }
}