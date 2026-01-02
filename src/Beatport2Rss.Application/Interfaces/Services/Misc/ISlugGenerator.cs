using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;

namespace Beatport2Rss.Application.Interfaces.Services.Misc;

public interface ISlugGenerator
{
    Slug Generate(FeedName feedName);
}