using Beatport2Rss.Domain.Common;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Contracts.Interfaces;

public interface ISlugGenerator
{
    Slug Generate(FeedName feedName);
    Slug Generate(Username username);
}