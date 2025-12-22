using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Services;

public interface ISlugGenerator
{
    Slug Generate(FeedName feedName);
    Slug Generate(Username username);
}