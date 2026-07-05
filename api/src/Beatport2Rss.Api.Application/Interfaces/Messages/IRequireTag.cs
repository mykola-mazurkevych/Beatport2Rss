using Beatport2Rss.Api.Domain.Common.ValueObjects;

namespace Beatport2Rss.Api.Application.Interfaces.Messages;

public interface IRequireTag
{
    Slug TagSlug { get; }
}