using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;

namespace Beatport2Rss.Domain.Users;

public sealed partial class User
{
    private readonly HashSet<Feed> _feeds = [];

    public IReadOnlySet<Feed> Feeds => _feeds.AsReadOnly();

    public bool HasFeed(FeedName name) =>
        _feeds.Any(f => f.Name == name);

    public void AddFeed(Feed feed) =>
        _feeds.Add(feed);

    public void RemoveFeed(Slug slug) =>
        _feeds.RemoveWhere(f => f.Slug == slug);

    public void UpdateFeedStatus(Slug slug, bool isActive) =>
        _feeds.Single(f => f.Slug == slug).UpdateStatus(isActive ? FeedStatus.Active : FeedStatus.Inactive);
}