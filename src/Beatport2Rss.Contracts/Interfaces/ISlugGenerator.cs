using Beatport2Rss.Domain.Common;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Contracts.Interfaces;

public interface ISlugGenerator
{
    Slug Generate(Username username);
}