using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;

namespace Beatport2Rss.Application.Interfaces.Services;

public interface ISlugGenerator
{
    Slug Generate(FeedName feedName);
}