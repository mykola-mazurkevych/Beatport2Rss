using Beatport2Rss.Common.Domain.Common;

namespace Beatport2Rss.Api.Application.Interfaces.Messages;

public interface IRequireTag
{
    Slug TagSlug { get; }
}