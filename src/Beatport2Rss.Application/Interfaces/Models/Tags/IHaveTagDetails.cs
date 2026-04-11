using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;

namespace Beatport2Rss.Application.Interfaces.Models.Tags;

public interface IHaveTagDetails
{
    TagId Id { get; }
    TagName Name { get; }
    Slug Slug { get; }
    DateTimeOffset CreatedAt { get; }
}