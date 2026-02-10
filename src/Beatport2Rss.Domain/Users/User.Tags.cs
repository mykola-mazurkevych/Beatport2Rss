using Beatport2Rss.Domain.Tags;

namespace Beatport2Rss.Domain.Users;

public sealed partial class User
{
    private readonly HashSet<Tag> _tags = [];

    public IReadOnlySet<Tag> Tags => _tags.AsReadOnly();

    public bool HasTag(TagName name) =>
        _tags.Any(t => t.Name == name);

    public void AddTag(Tag tag) =>
        _tags.Add(tag);
}