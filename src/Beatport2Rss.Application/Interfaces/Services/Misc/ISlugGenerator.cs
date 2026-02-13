using Beatport2Rss.Domain.Common.ValueObjects;

namespace Beatport2Rss.Application.Interfaces.Services.Misc;

public interface ISlugGenerator
{
    Slug Generate(string name);
}